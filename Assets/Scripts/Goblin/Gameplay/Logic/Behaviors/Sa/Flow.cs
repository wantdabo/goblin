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
        private enum ExecuteInstructType
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
        /// 管线内未满足条件的指令列表 - 后台
        /// </summary>
        private List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)> insidenotexebacks { get; set; }
        /// <summary>
        /// 管线内未满足条件的指令列表 - 前台
        /// </summary>
        private List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)> insidenotexefronts { get; set; }
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
            insidenotexebacks = ObjectCache.Ensure<List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)>>();
            insidenotexefronts = ObjectCache.Ensure<List<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)>>();
            Checkers();
            Executors();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            insidenotexebacks.Clear();
            ObjectCache.Set(insidenotexebacks);
            
            insidenotexefronts.Clear();
            ObjectCache.Set(insidenotexefronts);
            
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
        /// 检查管线激活状态
        /// </summary>
        /// <param name="flow">管线 Actor</param>
        /// <returns>Active (YES/NO)</returns>
        public bool CheckFlowActive(ulong flow)
        {
            if (false == stage.SeekBehaviorInfo(flow, out FlowInfo flowinfo)) return false;
            
            return flowinfo.active;
        }

        /// <summary>
        /// 生成管线
        /// </summary>
        /// <param name="owner">管线拥有者</param>
        /// <param name="pipelines">管线的 ID 列表, 用于指向管线数据</param>
        /// <param name="gen2run">生成并运行管线</param>
        /// <returns>Actor</returns>
        public ulong GenPipeline(ulong owner, List<uint> pipelines, bool gen2run = true)
        {
            var actor = stage.Spawn(new FlowPrefabInfo
            {
                owner = owner,
                pipelines = pipelines,
            });

            if (false == gen2run) return actor;
            Gen2RunPipeline(actor);

            return actor;
        }

        /// <summary>
        /// 生成转运行管线
        /// </summary>
        /// <param name="id">管线 ActorID</param>
        public void Gen2RunPipeline(ulong id)
        {
            RunPipeline(stage.GetBehaviorInfo<FlowInfo>(id));
            Spark(id, SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN);
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
                    ExecuteInstruct(ExecuteInstructType.Exit, pipelineid, index, instruct.data, flowinfo);
                }
                indexes.Clear();
                ObjectCache.Set(indexes);
            }
            flowinfo.active = false;
            // 结束管线
            stage.RmvActor(flowinfo.actor);
        }

        /// <summary>
        /// 触发花火
        /// </summary>
        /// <param name="actor">触发源</param>
        /// <param name="token">花火令牌</param>
        public void Spark(ulong actor, string token)
        {
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                if (false == flowinfo.active) continue;
                if (false == stage.cache.Valid(flowinfo.owner)) continue;
                
                // TODO : 优化查找
                foreach (var pipeline in flowinfo.pipelines)
                {
                    var data = PipelineDataReader.Read(pipeline);
                    if (null == data) continue;
                    for (int i = 0; i < data.sparkinstructs.Count; i++)
                    {
                        var instruct = data.sparkinstructs[i];
                        if (instruct.token != token) continue;
                        if (SPARK_INSTR_DEFINE.FLOW == instruct.influence && flowinfo.actor != actor) continue;
                        if (SPARK_INSTR_DEFINE.FLOW_OWNER == instruct.influence && flowinfo.owner != actor) continue;
                        
                        uint index = (uint)data.instructs.Count + (uint)i + 2;
                        ExecuteInstruct(ExecuteInstructType.Enter, pipeline, index, instruct.data, flowinfo);
                        ExecuteInstruct(ExecuteInstructType.Execute, pipeline, index, instruct.data, flowinfo);
                        ExecuteInstruct(ExecuteInstructType.Exit, pipeline, index, instruct.data, flowinfo);
                    }
                }
            }
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
                        if (isdoing) ExecuteInstruct(ExecuteInstructType.Exit, pipelineid, index, instruct.data, flowinfo);
                        continue;
                    }
                    
                    if (instruct.checkonce && isdoing)
                    {
                        ExecuteInstruct(ExecuteInstructType.Execute, pipelineid, index, instruct.data, flowinfo);
                        continue;
                    }
                    
                    // 指令进入 && 指令执行
                    if (false == CheckCondition(instruct.conditions, flowinfo))
                    {
                        // 如果指令不满足条件, 则记录下来, 以便后续处理
                        insidenotexebacks.Add((pipelineid, index, instruct, flowinfo));
                        continue;
                    }
                    
                    if (false == isdoing) ExecuteInstruct(ExecuteInstructType.Enter, pipelineid, index, instruct.data, flowinfo);
                    ExecuteInstruct(ExecuteInstructType.Execute, pipelineid, index, instruct.data, flowinfo);
                }
            }
        }
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                if (false == flowinfo.active) continue;
                if (false == stage.cache.Valid(flowinfo.owner))
                {
                    EndPipeline(flowinfo);
                    continue;
                }
                
                // 叠加持有者的 timescale
                FP flowtick = tick;
                if (stage.SeekBehaviorInfo(flowinfo.actor, out TickerInfo tickerinfo)) flowtick *= tickerinfo.timescale;
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
            InsideNotExeToExecute();
            // 检查管线信息, 如果管线的时间线超过了管线的长度, 则结束管线
            if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
            foreach (var flowinfo in flowinfos)
            {
                if (false == stage.cache.Valid(flowinfo.owner))
                {
                    EndPipeline(flowinfo);
                    continue;
                }
                
                if (flowinfo.active && flowinfo.timeline >= flowinfo.length) EndPipeline(flowinfo);
            }
        }
        
        /// <summary>
        /// 处理管线内未满足条件的指令
        /// </summary>
        private void InsideNotExeToExecute()
        {
            (insidenotexefronts, insidenotexebacks) = (insidenotexebacks, insidenotexefronts);
            foreach (var notexe in insidenotexefronts)
            {
                if (false == notexe.flowinfo.active) continue;
                if (false == CheckCondition(notexe.instruct.conditions, notexe.flowinfo)) continue;
                ExecuteInstruct(ExecuteInstructType.Enter, notexe.pipelineid, notexe.index, notexe.instruct.data, notexe.flowinfo);
                ExecuteInstruct(ExecuteInstructType.Execute, notexe.pipelineid, notexe.index, notexe.instruct.data, notexe.flowinfo);
            }
            insidenotexefronts.Clear();
            if (0 != insidenotexebacks.Count) InsideNotExeToExecute();
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
        /// <param name="data">指令数据</param>
        /// <param name="flowinfo">管线信息</param>
        /// <exception cref="Exception">未能找到相对应处理的指令执行器</exception>
        private void ExecuteInstruct(ExecuteInstructType type, uint pipelineid, uint index, InstructData data, FlowInfo flowinfo)
        {
            if (false == executors.TryGetValue(data.id, out var executor)) throw new Exception($"id : {data.id} cannot find executor.");
            if (false == flowinfo.doings.TryGetValue(pipelineid, out var indexes)) flowinfo.doings.Add(pipelineid, indexes = ObjectCache.Ensure<List<uint>>());
            
            switch (type)
            {
                case ExecuteInstructType.Enter:
                    executor.Enter((pipelineid, index), data, flowinfo);
                    if (false == indexes.Contains(index)) indexes.Add(index);
                    break;
                case ExecuteInstructType.Execute:
                    executor.Execute((pipelineid, index), data, flowinfo);
                    break;
                case ExecuteInstructType.Exit:
                    executor.Exit((pipelineid, index), data, flowinfo);
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
            Executor<RmvActorExecutor>(INSTR_DEFINE.RMV_ACTOR);
            Executor<ChangeStateExecutor>(INSTR_DEFINE.CHANGE_STATE);
        }
    }
}