using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;
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

        /// <summary>
        /// 生成管线
        /// </summary>
        /// <param name="owner">管线拥有者</param>
        /// <param name="pipelines">管线的 ID 列表, 用于指向管线数据</param>
        /// <returns>Actor</returns>
        public Actor GenPipeline(ulong owner, List<uint> pipelines)
        {
            return stage.Spawn<PipelinePrefab>(new PipelinePrefabInfo
            {
                owner = owner,
                pipelines = pipelines,
            });
        }
        
        /// <summary>
        /// 结束管线
        /// </summary>
        /// <param name="pipelineinfo">管线信息</param>
        public void EndPipeline(PipelineInfo pipelineinfo)
        {
            foreach (var pipelineid in pipelineinfo.pipelines)
            {
                var data = PipelineDataReader.LoadPipelineData(pipelineid);
                // 使用时间线最大值溢出的数值, 查询到所有指令, 如果该指令在执行中, 则退出
                if (false == data.Query(PIPELINE_DEFINE.OVERFLOW_LENGTH, out var instrinfos)) continue;
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    if (false == pipelineinfo.doings.TryGetValue(pipelineid, out var indexes) || false == indexes.Contains(instrinfo.index)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                }
            }
            
            // 结束管线
            stage.RmvActor(pipelineinfo.id);
        }

        /// <summary>
        /// 运行管线
        /// </summary>
        /// <param name="pipelineinfo">管线信息</param>
        private void RunPipeline(PipelineInfo pipelineinfo)
        {
            foreach (var pipelineid in pipelineinfo.pipelines)
            {
                var data = PipelineDataReader.LoadPipelineData(pipelineid);
                // 未找到改时间线可以执行的指令
                if (false == data.Query(pipelineinfo.timeline, out var instrinfos)) continue;
                
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    // 如果指令已经执行过, 则直接执行, 如果不在时间区间内则退出
                    if (pipelineinfo.doings.TryGetValue(pipelineid, out var indexes) && indexes.Contains(instrinfo.index))
                    {
                        ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                        if (false == instrinfo.inside) ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                        continue;
                    }
                    
                    // 指令进入 && 指令执行
                    if (false == instrinfo.inside) continue;
                    if (false == CheckCondition(instrinfo.instruct.conditions, pipelineinfo)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Enter, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                    ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, instrinfo.index, instrinfo.instruct, pipelineinfo);
                }
            }
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
                
                // 如果管线的时间线已经超过了管线的长度, 结束管线
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