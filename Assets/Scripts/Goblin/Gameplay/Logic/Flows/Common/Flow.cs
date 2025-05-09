using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管线流
    /// </summary>
    public class Flow : Behavior
    {
        /// <summary>
        /// 指令条件检查器列表
        /// </summary>
        private Dictionary<ushort, Checker> checkers { get; set; }
        /// <summary>
        /// 指令执行器列表
        /// </summary>
        private Dictionary<ushort, Executor> executors { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            Checkers();
            Executors();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            foreach (var kv in checkers)
            {
                kv.Value.Unload();
                ObjectCache.Set(kv.Value);
            }
            checkers.Clear();
            ObjectCache.Set(checkers);
            
            foreach (var kv in executors)
            {
                kv.Value.Unload();
                ObjectCache.Set(kv.Value);
            }
            executors.Clear();
            ObjectCache.Set(executors);
        }

        public void StartPipeline()
        {
        }

        /// <summary>
        /// 运行管线
        /// </summary>
        /// <param name="pipelineinfo">管线信息</param>
        private void RunPipeline(PipelineInfo pipelineinfo)
        {
            foreach (var pipelineid in pipelineinfo.pipelines)
            {
                // TODO 临时虚构一个, 后续要根据 ID 进行加载管线数据
                PipelineData data = null;
                if (false == data.Query(pipelineinfo.timeline, out var instrinfos)) continue;
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    if (pipelineinfo.doings.TryGetValue(pipelineid, out var indexes) && indexes.Contains(instrinfo.index))
                    {
                        ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                        if (false == instrinfo.inside) ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                        continue;
                    }
                    
                    if (false == instrinfo.inside) continue;
                    if (false == CheckCondition(instrinfo.instruct.conditions, pipelineinfo)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Enter, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                    ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                }
            }
        }
        
        /// <summary>
        /// 结束管线
        /// </summary>
        /// <param name="pipelineinfo">管线信息</param>
        private void EndPipeline(PipelineInfo pipelineinfo)
        {
            foreach (var pipelineid in pipelineinfo.pipelines)
            {
                // TODO 临时虚构一个, 后续要根据 ID 进行加载管线数据
                PipelineData data = null;
                if (false == data.Query(PIPELINE_DEFINE.OVERFLOW_LENGTH, out var instrinfos)) continue;
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    if (false == pipelineinfo.doings.TryGetValue(pipelineid, out var indexes) || false == indexes.Contains(instrinfo.index)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                }
            }
            stage.RmvActor(pipelineinfo.id);
        }
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos<PipelineInfo>(out var pipelineinfos)) return;
            foreach (var pipelineinfo in pipelineinfos)
            {
                // 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
                pipelineinfo.elapsed += (tick * stage.cfg.fp2int).AsUInt();
                while (pipelineinfo.elapsed >= GAME_DEFINE.LOGIC_TICK_MS)
                {
                    RunPipeline(pipelineinfo);
                    pipelineinfo.timeline += GAME_DEFINE.LOGIC_TICK_MS;
                    pipelineinfo.elapsed -= GAME_DEFINE.LOGIC_TICK_MS;
                }
                if (pipelineinfo.timeline >= pipelineinfo.length) EndPipeline(pipelineinfo);
            }

            pipelineinfos.Clear();
            ObjectCache.Set(pipelineinfos);
        }

        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="conditions">条件列表</param>
        /// <param name="pipelineinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        /// <exception cref="Exception">未能找到相对应处理的指令执行条件检查器</exception>
        private bool CheckCondition(List<Condition> conditions, PipelineInfo pipelineinfo)
        {
            foreach (var condition in conditions)
            {
                if (false == checkers.TryGetValue(condition.id, out var checker)) throw new Exception($"id : {condition.id} cannot find checker.");
                if (false == checker.Check(condition, pipelineinfo)) return false;
            }

            return true;
        }

        /// <summary>
        /// 指令执行类型
        /// </summary>
        private enum ExecuteInstrucType
        {
            /// <summary>
            /// 进入
            /// </summary>
            Enter,
            /// <summary>
            /// 执行
            /// </summary>
            Execute,
            /// <summary>
            /// 退出
            /// </summary>
            Exit,
        }
        
        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="type">指令执行类型</param>
        /// <param name="pipelineid">管线 ID</param>
        /// <param name="index">指令索引</param>
        /// <param name="instruct">指令</param>
        /// <param name="pipelineinfo">管线信息</param>
        /// <exception cref="Exception">未能找到相对应处理的指令执行器</exception>
        private void ExecuteInstruct(ExecuteInstrucType type, uint pipelineid, uint index, Instruct instruct, PipelineInfo pipelineinfo)
        {
            if (false == executors.TryGetValue(instruct.id, out var executor)) throw new Exception($"id : {instruct.id} cannot find executor.");
            if (false == pipelineinfo.doings.TryGetValue(pipelineid, out var indexes)) pipelineinfo.doings.Add(pipelineid, indexes = ObjectCache.Get<List<uint>>());
            
            switch (type)
            {
                case ExecuteInstrucType.Enter:
                    executor.Enter(instruct, pipelineinfo);
                    if (false == indexes.Contains(index)) indexes.Add(index);
                    break;
                case ExecuteInstrucType.Execute:
                    executor.Execute(instruct, pipelineinfo);
                    break;
                case ExecuteInstrucType.Exit:
                    executor.Exit(instruct, pipelineinfo);
                    if (indexes.Contains(index)) indexes.Remove(index);
                    break;
            }
        }

        /// <summary>
        /// 初始化指令条件检查器
        /// </summary>
        private void Checkers()
        {
            checkers = ObjectCache.Get<Dictionary<ushort, Checker>>();
        }

        /// <summary>
        /// 初始化指令执行器
        /// </summary>
        private void Executors()
        {
            executors = ObjectCache.Get<Dictionary<ushort, Executor>>();
        }
    }
}