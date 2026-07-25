using System;
using System.Collections.Generic;
using Goblin.Common;
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

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

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
    private GBLList<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)> insidenotexebacks { get; set; }
    /// <summary>
    /// 管线内未满足条件的指令列表 - 前台
    /// </summary>
    private GBLList<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)> insidenotexefronts { get; set; }
    /// <summary>
    /// 指令条件检查器列表
    /// </summary>
    private GBLDict<ushort, Checker> checkers { get; set; }
    /// <summary>
    /// 指令执行器列表
    /// </summary>
    private GBLDict<ushort, Executor> executors { get; set; }
    /// <summary>
    /// 指令执行器字典
    /// </summary>
    private GBLDict<Type, Executor> executordict { get; set; }
    /// <summary>
    /// 火花索引（token → pipelineid → SparkInstruct 列表）
    /// </summary>
    private GBLDict<string, GBLDict<uint, GBLList<SparkInstruct>>> sparkindex { get; set; }
    /// <summary>
    /// 已索引的管线 ID 集合
    /// </summary>
    private GBLHashSet<uint> indexedpipelines { get; set; }

    protected override void OnAssemble()
    {
        base.OnAssemble();
        insidenotexebacks = ObjectCache.Ensure<GBLList<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)>>();
        insidenotexefronts = ObjectCache.Ensure<GBLList<(uint pipelineid, uint index, Instruct instruct, FlowInfo flowinfo)>>();
        sparkindex = ObjectCache.Ensure<GBLDict<string, GBLDict<uint, GBLList<SparkInstruct>>>>();
        indexedpipelines = ObjectCache.Ensure<GBLHashSet<uint>>();
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
            
        foreach (var kv in sparkindex)
        {
            foreach (var list in kv.Value.Values)
            {
                list.Clear();
                ObjectCache.Set(list);
            }
            kv.Value.Clear();
            ObjectCache.Set(kv.Value);
        }
        sparkindex.Clear();
        ObjectCache.Set(sparkindex);
        indexedpipelines.Clear();
        ObjectCache.Set(indexedpipelines);

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
        executordict.Clear();
        ObjectCache.Set(executordict);
    }

    /// <summary>
    /// 生成管线
    /// </summary>
    /// <param name="owner">管线拥有者</param>
    /// <param name="pipelines">管线的 ID 列表, 用于指向管线数据</param>
    /// <param name="gen2run">生成并运行管线</param>
    /// <returns>Actor</returns>
    public ulong GenPipeline(ulong owner, IEnumerable<uint> pipelines, bool gen2run = true)
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
            GBLList<uint> indexes = ObjectCache.Ensure<GBLList<uint>>();
            indexes.AddRange(list);
            foreach (var index in indexes)
            {
                if (false == data.Query(index, out var instruct)) continue;
                ExecuteInstruct(ExecuteInstructType.Exit, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
            }
            indexes.Clear();
            ObjectCache.Set(indexes);
        }
        // 结束管线
        stage.RmvActor(flowinfo.actor);
    }
        
    /// <summary>
    /// 触发火花
    /// </summary>
    /// <param name="flowinfo">管线信息</param>
    /// <param name="influence">触发范围</param>
    /// <param name="token">火花令牌</param>
    public void Spark(FlowInfo flowinfo, sbyte influence, string token)
    {
        switch (influence)
        {
            case SPARK_INSTR_DEFINE.FLOW:
                Spark(flowinfo.actor, token);
                break;
            case SPARK_INSTR_DEFINE.FLOW_OWNER:
                Spark(flowinfo.owner, token);
                break;
        }
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

            foreach (var pipelineid in flowinfo.pipelines)
            {
                if (false == indexedpipelines.Contains(pipelineid))
                {
                    var data = PipelineDataReader.Read(pipelineid);
                    if (null != data) IndexPipeline(pipelineid, data);
                    indexedpipelines.Add(pipelineid);
                }
                if (false == sparkindex.TryGetValue(token, out var pipelinemap)) continue;
                if (false == pipelinemap.TryGetValue(pipelineid, out var instructs)) continue;

                var curdata = PipelineDataReader.Read(pipelineid);
                for (int i = 0; i < instructs.Count; i++)
                {
                    var instruct = instructs[i];
                    if (SPARK_INSTR_DEFINE.FLOW == instruct.influence && flowinfo.actor != actor) continue;
                    if (SPARK_INSTR_DEFINE.FLOW_OWNER == instruct.influence && flowinfo.owner != actor) continue;

                    uint index = (uint)curdata.instructs.Count + (uint)i + 2;
                    if (false == ExecuteInstruct(ExecuteInstructType.Enter, pipelineid, index, instruct.data, instruct.conditions, flowinfo)) continue;
                    ExecuteInstruct(ExecuteInstructType.Execute, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
                    ExecuteInstruct(ExecuteInstructType.Exit, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
                }
            }
        }
    }

    /// <summary>
    /// 索引管线的火花指令
    /// </summary>
    /// <param name="pipelineid">管线 ID</param>
    /// <param name="data">管线数据</param>
    private void IndexPipeline(uint pipelineid, PipelineData data)
    {
        foreach (var instruct in data.sparkinstructs)
        {
            if (false == sparkindex.TryGetValue(instruct.token, out var pipelinemap))
                sparkindex.Add(instruct.token, pipelinemap = ObjectCache.Ensure<GBLDict<uint, GBLList<SparkInstruct>>>());
            if (false == pipelinemap.TryGetValue(pipelineid, out var list))
                pipelinemap.Add(pipelineid, list = ObjectCache.Ensure<GBLList<SparkInstruct>>());
            list.Add(instruct);
        }
    }

    /// <summary>
    /// 获取指令执行器
    /// </summary>
    /// <typeparam name="T">指令执行器类型</typeparam>
    /// <returns>指令执行器</returns>
    /// <exception cref="Exception">未能找到相应的指令执行器</exception>
    public T Executor<T>() where T : Executor
    {
        if (false == executordict.TryGetValue(typeof(T), out var executor)) throw new Exception("cannot find executor.");

        return executor as T;
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

            flowinfo.completedindex.TryGetValue(pipelineid, out var lastcompleted);
            uint index = 0;
            foreach (var instruct in data.instructs)
            {
                index++;
                if (false == flowinfo.active) continue;
                if (index <= lastcompleted) continue;
                if (instruct.begin > flowinfo.timeline) break;
                if (instruct.end < flowinfo.timeline)
                {
                    // 仅连续推进，防止跳过仍在活跃的指令
                    if (index == lastcompleted + 1) flowinfo.completedindex[pipelineid] = index;
                    continue;
                }

                flowinfo.doings.TryGetValue(pipelineid, out var indexes);
                // 管线已经进入, 正在运行中
                var isdoing = null != indexes && indexes.Contains(index);
                // 在时间区间内
                var inside = instruct.begin <= flowinfo.timeline && instruct.end >= flowinfo.timeline;

                // 如果不在时间区间内则退出
                if (false == inside)
                {
                    if (isdoing) ExecuteInstruct(ExecuteInstructType.Exit, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
                    continue;
                }
                    
                if (instruct.checkonce && isdoing)
                {
                    ExecuteInstruct(ExecuteInstructType.Execute, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
                    continue;
                }
                    
                // 指令进入 && 指令执行
                var entered = false;
                if (false == isdoing) entered = ExecuteInstruct(ExecuteInstructType.Enter, pipelineid, index, instruct.data, instruct.conditions, flowinfo);

                if (false == (entered || isdoing))
                {
                    // 如果指令不满足条件, 则记录下来, 以便后续处理
                    insidenotexebacks.Add((pipelineid, index, instruct, flowinfo));
                    continue;
                }
                    
                ExecuteInstruct(ExecuteInstructType.Execute, pipelineid, index, instruct.data, instruct.conditions, flowinfo);
            }
        }
    }
        
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehaviorInfos<FlowInfo>(out var flowinfos)) return;
        var queue = ObjectCache.Ensure<Queue<FlowInfo>, FlowInfo>(CAPACITY_DEFINE.L3);
        foreach (var flowinfo in flowinfos)
        {
            // 叠加持有者的 timescale
            FP flowtick = tick;
            if (stage.SeekBehaviorInfo(flowinfo.owner, out TickerInfo tickerinfo)) flowtick *= tickerinfo.timescale;
            flowinfo.framepass += (flowtick * stage.cfg.fp2int).AsUInt();
            queue.Enqueue(flowinfo);
        }
            
        while (queue.TryDequeue(out var flowinfo))
        {
            if (false == flowinfo.active) continue;
            if (false == stage.cache.Valid(flowinfo.owner))
            {
                EndPipeline(flowinfo);
                continue;
            }

            if (flowinfo.framepass >= GAME_DEFINE.LOGIC_TICK_MS)
            {
                flowinfo.timeline += GAME_DEFINE.LOGIC_TICK_MS;
                flowinfo.framepass -= GAME_DEFINE.LOGIC_TICK_MS;
                RunPipeline(flowinfo);
                // 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
                if (flowinfo.framepass >= GAME_DEFINE.LOGIC_TICK_MS) queue.Enqueue(flowinfo);
            }
        }

        queue.Clear();
        ObjectCache.Set(queue);
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
    private void InsideNotExeToExecute(int depth = 0)
    {
        if (FLOW_DEFINE.MAX_INSIDE_NOTEXE_DEPTH <= depth) return;
        (insidenotexefronts, insidenotexebacks) = (insidenotexebacks, insidenotexefronts);
        foreach (var notexe in insidenotexefronts)
        {
            if (false == notexe.flowinfo.active) continue;
            if (false == ExecuteInstruct(ExecuteInstructType.Enter, notexe.pipelineid, notexe.index, notexe.instruct.data, notexe.instruct.conditions, notexe.flowinfo)) continue;
            ExecuteInstruct(ExecuteInstructType.Execute, notexe.pipelineid, notexe.index, notexe.instruct.data, notexe.instruct.conditions, notexe.flowinfo);
        }
        insidenotexefronts.Clear();
        if (0 != insidenotexebacks.Count) InsideNotExeToExecute(depth + 1);
    }

    /// <summary>
    /// 检查指令条件
    /// </summary>
    /// <param name="data">指令数据</param>
    /// <param name="conditions">条件列表</param>
    /// <param name="flowinfo">管线信息</param>
    /// <param name="target">执行目标</param>
    /// <returns>YES/NO</returns>
    /// <exception cref="Exception">未能找到相对应处理的指令执行条件检查器</exception>
    private bool CheckCondition(InstructData data, GBLList<Condition> conditions, FlowInfo flowinfo, ulong target)
    {
        foreach (var condition in conditions)
        {
            if (false == checkers.TryGetValue(condition.id, out var checker)) throw new Exception($"id : {condition.id} cannot find checker.");
            if (false == checker.Check(condition, flowinfo, target)) return false;
        }

        return true;
    }
        
    /// <summary>
    /// 根据 ET 枚举搜索目标
    /// </summary>
    /// <param name="flowinfo">管线信息</param>
    /// <param name="et">执行目标类型</param>
    /// <returns>目标 ActorID，搜索失败返回 0</returns>
    public ulong SeekETTarget(FlowInfo flowinfo, byte et)
    {
        switch (et)
        {
            case FLOW_DEFINE.ET_FLOW:       return flowinfo.actor;
            case FLOW_DEFINE.ET_FLOW_OWNER: return flowinfo.owner;
            case FLOW_DEFINE.ET_CASTER:
            {
                var current = flowinfo.owner;
                for (var depth = 0; depth < FLOW_DEFINE.MAX_CASTER_SEARCH_DEPTH; depth++)
                {
                    if (stage.SeekBehavior(current, out Tag tag) && tag.Get(TAG_DEFINE.ACTOR_TYPE, out var actortype))
                    {
                        if (ACTOR_DEFINE.CASTER_TYPES.Contains((byte)actortype)) return current;
                        if (ACTOR_DEFINE.NONE == actortype || ACTOR_DEFINE.STAGE == actortype) return 0;
                    }
                    if (stage.SeekBehaviorInfo(current, out MagicInfo magic)) { current = magic.owner; continue; }
                    if (stage.SeekBehaviorInfo(current, out BuffInfo buff)) { current = buff.owner; continue; }
                    return 0;
                }
                return 0;
            }
            default: return 0;
        }
    }

    /// <summary>
    /// 执行指令
    /// </summary>
    /// <param name="type">指令执行类型</param>
    /// <param name="pipelineid">管线 ID</param>
    /// <param name="index">指令索引</param>
    /// <param name="data">指令数据</param>
    /// <param name="conditions">指令条件</param>
    /// <param name="flowinfo">管线信息</param>
    /// <exception cref="Exception">未能找到相对应处理的指令执行器</exception>
    /// <returns>是否至少有一个目标成功执行</returns>
    private bool ExecuteInstruct(ExecuteInstructType type, uint pipelineid, uint index, InstructData data, GBLList<Condition> conditions, FlowInfo flowinfo)
    {
        if (false == executors.TryGetValue(data.id, out var executor)) throw new Exception($"id : {data.id} cannot find executor.");
        if (false == flowinfo.doings.TryGetValue(pipelineid, out var indexes)) flowinfo.doings.Add(pipelineid, indexes = ObjectCache.Ensure<GBLList<uint>>());

        var executed = false;

        void Do(ulong target)
        {
            if (type != ExecuteInstructType.Exit && false == CheckCondition(data, conditions, flowinfo, target)) return;

            switch (type)
            {
                case ExecuteInstructType.Enter:
                    executor.Enter((pipelineid, index), data, flowinfo, target);
                    if (false == indexes.Contains(index)) indexes.Add(index);
                    break;
                case ExecuteInstructType.Execute:
                    executor.Execute((pipelineid, index), data, flowinfo, target);
                    break;
                case ExecuteInstructType.Exit:
                    executor.Exit((pipelineid, index), data, flowinfo, target);
                    if (indexes.Contains(index)) indexes.Remove(index);
                    break;
            }

            executed = true;
        }
            
        switch (data.et)
        {
            case FLOW_DEFINE.ET_FLOW_HIT:
            case FLOW_DEFINE.ET_HIT_VICTIM:
                if (stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo flowcollision))
                    foreach (var target in flowcollision.targets) Do(target.actor);
                break;
            default:
                Do(SeekETTarget(flowinfo, data.et));
                break;
        }

        return executed;
    }

    /// <summary>
    /// 初始化指令条件检查器
    /// </summary>
    private void Checkers()
    {
        checkers = ObjectCache.Ensure<GBLDict<ushort, Checker>>();
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
        executors = ObjectCache.Ensure<GBLDict<ushort, Executor>>();
        executordict = ObjectCache.Ensure<GBLDict<Type, Executor>>();
        void Executor<T>(ushort id) where T : Executor, new()
        {
            var executor = ObjectCache.Ensure<T>().Load(stage);
            executors.Add(id, executor);
            executordict.Add(typeof(T), executor);
        }
            
        Executor<AnimationExecutor>(INSTR_DEFINE.ANIMATION);
        Executor<SpatialPositionExecutor>(INSTR_DEFINE.SPATIAL_POSITION);
        Executor<CreateMagicExecutor>(INSTR_DEFINE.CREATE_MAGIC);

        Executor<LaunchSkillExecutor>(INSTR_DEFINE.LAUNCH_SKILL);
        Executor<EffectExecutor>(INSTR_DEFINE.EFFECT);
        Executor<CollisionExecutor>(INSTR_DEFINE.COLLISION);
        Executor<RmvActorExecutor>(INSTR_DEFINE.RMV_ACTOR);
        Executor<ChangeStateExecutor>(INSTR_DEFINE.CHANGE_STATE);
        Executor<SparkExecutor>(INSTR_DEFINE.SPARK);
        Executor<HitLagExecutor>(INSTR_DEFINE.HIT_LAG);
        Executor<TimeScaleExecutor>(INSTR_DEFINE.TIMESCALE);
        Executor<BeHitExecutor>(INSTR_DEFINE.BEHIT);
        Executor<SkillBreakExecutor>(INSTR_DEFINE.SKILLBREAK);
        Executor<DamageExecutor>(INSTR_DEFINE.DAMAGE);
        Executor<SoundExecutor>(INSTR_DEFINE.SOUND);
    }
}
