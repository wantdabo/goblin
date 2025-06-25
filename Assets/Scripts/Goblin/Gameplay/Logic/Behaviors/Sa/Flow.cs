using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Checkers;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Prefabs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// 管线流
    /// </summary>
    public class Flow : Behavior
    {
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
        /// 管线内未满足条件的指令列表
        /// </summary>
        private List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)> insidenotexes { get; set; }
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
            insidenotexes = ObjectCache.Ensure<List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)>>();
            Checkers();
            Executors();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            insidenotexes.Clear();
            ObjectCache.Set(insidenotexes);
            
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
        public ulong GenPipeline(ulong owner, List<uint> pipelines)
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
            if (false == flowinfo.active) return;
            
            foreach (var pipelineid in flowinfo.pipelines)
            {
                var data = PipelineDataReader.Read(pipelineid);
                if (false == flowinfo.doings.TryGetValue(pipelineid, out var list)) continue;
                List<uint> indexes = ObjectCache.Ensure<List<uint>>();
                indexes.AddRange(list);
                foreach (var index in indexes)
                {
                    if (false == data.Query(index, out var instruct)) continue;
                    ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, index, instruct, flowinfo);
                }
                indexes.Clear();
                ObjectCache.Set(indexes);
            }
            flowinfo.active = false;
            // 结束管线
            stage.RmvActor(flowinfo.actor);
        }

        /// <summary>
        /// 运行管线
        /// </summary>
        /// <param name="flowinfo">管线信息</param>
        private void RunPipeline(FlowInfo flowinfo)
        {
            foreach (var pipelineid in flowinfo.pipelines)
            {
                if (false == flowinfo.active) continue;
                
                var data = PipelineDataReader.Read(pipelineid);
                // 未找到改时间线可以执行的指令
                if (null == data || 0 == data.instructs.Count) continue;

                uint index = 0;
                foreach (var instruct in data.instructs)
                {
                    index++;
                    if (false == flowinfo.active) continue;
                    if (instruct.begin > flowinfo.timeline) break;
                    
                    flowinfo.doings.TryGetValue(pipelineid, out var indexes);
                    // 管线已经进入, 正在运行中
                    var isdoing = null != indexes && indexes.Contains(index);
                    // 在时间区间内
                    var inside = instruct.begin <= flowinfo.timeline && instruct.end > flowinfo.timeline;
                    
                    // 如果不在时间区间内则退出
                    if (false == inside)
                    {
                        if (isdoing) ExecuteInstruct(ExecuteInstrucType.Exit, pipelineid, index, instruct, flowinfo);
                        continue;
                    }
                    
                    if (instruct.checkonce && isdoing)
                    {
                        ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, index, instruct, flowinfo);
                        continue;
                    }
                    
                    // 指令进入 && 指令执行
                    if (false == CheckCondition(instruct.conditions, flowinfo))
                    {
                        // 如果指令不满足条件, 则记录下来, 以便后续处理
                        insidenotexes.Add((pipelineid, index, instruct, flowinfo));
                        continue;
                    }
                    
                    if (false == isdoing) ExecuteInstruct(ExecuteInstrucType.Enter, pipelineid, index, instruct, flowinfo);
                    ExecuteInstruct(ExecuteInstrucType.Execute, pipelineid, index, instruct, flowinfo);
                }
            }
        }
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                if (stage.cache.rmvactors.Contains(flowinfo.owner)) continue;
                if (false == flowinfo.active) continue;
                
                // 叠加持有者的 timescale
                FP flowtick = tick;
                if (stage.SeekBehaviorInfo(flowinfo.owner, out TickerInfo ownertickerinfo)) flowtick *= ownertickerinfo.timescale;
                // 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
                flowinfo.framepass += (flowtick * stage.cfg.fp2int).AsUInt();
                while (flowinfo.framepass >= GAME_DEFINE.LOGIC_TICK_MS)
                {
                    RunPipeline(flowinfo);
                    flowinfo.timeline += GAME_DEFINE.LOGIC_TICK_MS;
                    flowinfo.framepass -= GAME_DEFINE.LOGIC_TICK_MS;
                }
            }
        }

        protected override void OnEndTick()
        {
            base.OnEndTick();
            // 处理指令条件不满足的指令
            foreach (var notexe in insidenotexes)
            {
                if (false == notexe.flowinfo.active) continue;
                if (false == CheckCondition(notexe.instruct.conditions, notexe.flowinfo)) continue;
                ExecuteInstruct(ExecuteInstrucType.Enter, notexe.pipelineid, notexe.index, notexe.instruct, notexe.flowinfo);
                ExecuteInstruct(ExecuteInstrucType.Execute, notexe.pipelineid, notexe.index, notexe.instruct, notexe.flowinfo);
            }
            insidenotexes.Clear();
            
            // 检查管线信息, 如果管线的时间线超过了管线的长度, 则结束管线
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                if (stage.cache.rmvactors.Contains(flowinfo.owner))
                {
                    EndPipeline(flowinfo);
                    continue;
                }
                
                if (flowinfo.active && flowinfo.timeline >= flowinfo.length) EndPipeline(flowinfo);
            }
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
            if (false == executors.TryGetValue(instruct.data.id, out var executor)) throw new Exception($"id : {instruct.data.id} cannot find executor.");
            if (false == flowinfo.doings.TryGetValue(pipelineid, out var indexes)) flowinfo.doings.Add(pipelineid, indexes = ObjectCache.Ensure<List<uint>>());
            
            switch (type)
            {
                case ExecuteInstrucType.Enter:
                    executor.Enter((pipelineid, index), instruct.data, flowinfo);
                    if (false == indexes.Contains(index)) indexes.Add(index);
                    break;
                case ExecuteInstrucType.Execute:
                    executor.Execute((pipelineid, index), instruct.data, flowinfo);
                    break;
                case ExecuteInstrucType.Exit:
                    executor.Exit((pipelineid, index), instruct.data, flowinfo);
                    if (indexes.Contains(index)) indexes.Remove(index);
                    break;
            }
        }

        /// <summary>
        /// 初始化指令条件检查器
        /// </summary>
        private void Checkers()
        {
            checkers = ObjectCache.Ensure<Dictionary<ushort, Checker>>();
            void Checker<T>(ushort id) where T : Checker, new()
            {
                checkers.Add(id, ObjectCache.Ensure<T>().Load(stage));
            }
            
            Checker<InputChecker>(CONDITION_DEFINE.INPUT);
        }

        /// <summary>
        /// 初始化指令执行器
        /// </summary>
        private void Executors()
        {
            executors = ObjectCache.Ensure<Dictionary<ushort, Executor>>();
            void Executor<T>(ushort id) where T : Executor, new()
            {
                executors.Add(id, ObjectCache.Ensure<T>().Load(stage));
            }
            
            Executor<AnimationExecutor>(INSTR_DEFINE.ANIMATION);
            Executor<SpatialPositionExecutor>(INSTR_DEFINE.SPATIAL_POSITION);
            Executor<CreateBulletExecutor>(INSTR_DEFINE.CREATE_BULLET);
            Executor<BulletMotionExecutor>(INSTR_DEFINE.BULLET_MOTION);
            Executor<LaunchSkillExecutor>(INSTR_DEFINE.LAUNCH_SKILL);
            Executor<EffectExecutor>(INSTR_DEFINE.EFFECT);
            Executor<CollisionExecutor>(INSTR_DEFINE.COLLISION);
        }
    }
}