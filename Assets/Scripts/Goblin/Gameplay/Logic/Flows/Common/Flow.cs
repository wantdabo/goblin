using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
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
            var actor = stage.Spawn(new FlowPrefabInfo
            {
                owner = owner,
                pipelines = pipelines,
            });

            return actor;
        }

        /// <summary>
        /// 结束管线
        /// </summary>
        /// <param name="id">管线 ActorID</param>
        public void EndPipeline(ulong id)
        {
            if (false == stage.SeekBehaviorInfo(id, out FlowInfo flowinfo)) return;
            EndPipeline(flowinfo);
        }

        /// <summary>
        /// 结束管线
        /// </summary>
        /// <param name="flowinfo">管线信息</param>
        public void EndPipeline(FlowInfo flowinfo)
        {
            foreach (var pipelineid in flowinfo.pipelines)
            {
                var data = PipelineDataReader.ReadPipelineData(pipelineid);
                // 使用时间线最大值溢出的数值, 查询到所有指令, 如果该指令在执行中, 则退出
                if (false == data.Query(FLOW_DEFINE.OVERFLOW_LENGTH, out var instrinfos)) continue;
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    if (false == flowinfo.doings.TryGetValue(pipelineid, out var indexes) || false == indexes.Contains(instrinfo.index)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, flowinfo);
                }
            }
            // 结束管线
            stage.RmvActor(flowinfo.id);
        }

        /// <summary>
        /// 运行管线
        /// </summary>
        /// <param name="flowinfo">管线信息</param>
        private void RunPipeline(FlowInfo flowinfo)
        {
            foreach (var pipelineid in flowinfo.pipelines)
            {
                var data = PipelineDataReader.ReadPipelineData(pipelineid);
                // 未找到改时间线可以执行的指令
                if (false == data.Query(flowinfo.timeline, out var instrinfos)) continue;
                
                foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                {
                    flowinfo.doings.TryGetValue(pipelineid, out var indexes);
                    // 如果不在时间区间内则退出
                    if (false == instrinfo.inside)
                    {
                        if (null != indexes && indexes.Contains(instrinfo.index)) ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, instrinfo.index, instrinfo.instruct, flowinfo);

                        continue;
                    }
                    
                    // 指令进入 && 指令执行
                    if (false == CheckCondition(instrinfo.instruct.conditions, flowinfo)) continue;
                    if (null == indexes || false == indexes.Contains(instrinfo.index)) ExecuteInstruct(ExecuteInstrucType.Enter, pipelineid, instrinfo.index, instrinfo.instruct, flowinfo);

                    ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, instrinfo.index, instrinfo.instruct, flowinfo);
                }
            }
        }
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                // 叠加持有者的 timescale
                if (stage.SeekBehaviorInfo(flowinfo.owner, out TickerInfo ownertickerinfo)) tick *= ownertickerinfo.timescale;
                // 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
                flowinfo.elapsed += (tick * stage.cfg.fp2int).AsUInt();
                while (flowinfo.elapsed >= GAME_DEFINE.LOGIC_TICK_MS)
                {
                    RunPipeline(flowinfo);
                    flowinfo.timeline += GAME_DEFINE.LOGIC_TICK_MS;
                    flowinfo.elapsed -= GAME_DEFINE.LOGIC_TICK_MS;
                }
                
                // 如果管线的时间线已经超过了管线的长度, 结束管线
                if (flowinfo.timeline >= flowinfo.length) EndPipeline(flowinfo);
            }

            flowinfos.Clear();
            ObjectCache.Set(flowinfos);
        }

        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="conditions">条件列表</param>
        /// <param name="flowinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        /// <exception cref="Exception">未能找到相对应处理的指令执行条件检查器</exception>
        private bool CheckCondition(List<Condition> conditions, FlowInfo flowinfo)
        {
            foreach (var condition in conditions)
            {
                if (false == checkers.TryGetValue(condition.id, out var checker)) throw new Exception($"id : {condition.id} cannot find checker.");
                if (false == checker.Check(condition, flowinfo)) return false;
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
        /// <param name="flowinfo">管线信息</param>
        /// <exception cref="Exception">未能找到相对应处理的指令执行器</exception>
        private void ExecuteInstruct(ExecuteInstrucType type, uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)
        {
            if (false == executors.TryGetValue(instruct.id, out var executor)) throw new Exception($"id : {instruct.id} cannot find executor.");
            if (false == flowinfo.doings.TryGetValue(pipelineid, out var indexes)) flowinfo.doings.Add(pipelineid, indexes = ObjectCache.Get<List<uint>>());
            
            switch (type)
            {
                case ExecuteInstrucType.Enter:
                    executor.Enter(instruct, flowinfo);
                    if (false == indexes.Contains(index)) indexes.Add(index);
                    break;
                case ExecuteInstrucType.Execute:
                    executor.Execute(instruct, flowinfo);
                    break;
                case ExecuteInstrucType.Exit:
                    executor.Exit(instruct, flowinfo);
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