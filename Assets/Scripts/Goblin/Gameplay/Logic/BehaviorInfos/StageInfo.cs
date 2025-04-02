using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 场景状态
    /// </summary>
    public enum StageState
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 初始化
        /// </summary>
        Initialized,
        /// <summary>
        /// 销毁了
        /// </summary>
        Disposed,
        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
        /// <summary>
        /// 驱动中
        /// </summary>
        Ticking,
        /// <summary>
        /// 停止了
        /// </summary>
        Stopped,
    }
    
    public class StageInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前 Stage 状态
        /// </summary>
        public StageState state { get; set; } = StageState.None;
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        /// <summary>
        /// 帧步长, 结果包含 timescale 影响
        /// </summary>
        public FP tick => GAME_DEFINE.LOGIC_TICK * timescale;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP timescale { get; set; }
        /// <summary>
        /// Actor 自增 ID
        /// </summary>
        public ulong increment { get; set; }
        /// <summary>
        /// Actor 列表
        /// </summary>
        public List<ulong> actors { get; set; }
        /// <summary>
        /// RmvActor 列表
        /// </summary>
        public List<ulong> rmvactors { get; set; }
        /// <summary>
        /// 行为列表, 键为 ActorID, 值为该 Actor 上的所有行为类型
        /// </summary>
        public Dictionary<ulong, List<Type>> behaviors { get; set; }
        /// <summary>
        /// 行为所有者列表, 键为行为类型, 值为该行为类型所拥有的所有 ActorID 列表
        /// </summary>
        public Dictionary<Type, List<ulong>> behaviorowners { get; set; }
        /// <summary>
        /// 行为信息列表, 键为 ActorID, 值为该 Actor 上的所有行为信息
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviordict { get; set; }
        /// <summary>
        /// 行为信息列表, 键为行为类型, 值为该行为类型的所有 BehaviorInfo 列表
        /// </summary>
        public Dictionary<Type, List<BehaviorInfo>> behaviorinfos { get; set; }
        /// <summary>
        /// 已组装的 Actor 列表, 键为 ActorID, 值为 Actor 实例
        /// </summary>
        public Dictionary<ulong, Actor> actorassembleds { get; set; }
        /// <summary>
        /// 已组装的行为列表, 键为 ActorID, 值为该 Actor 上的所有已组装的行为实例
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, Behavior>> behaviorassembleds { get; set; }
        
        protected override void OnReady()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            actors = ObjectCache.Get<List<ulong>>();
            rmvactors = ObjectCache.Get<List<ulong>>();
            behaviors = ObjectCache.Get<Dictionary<ulong, List<Type>>>();
            behaviorowners = ObjectCache.Get<Dictionary<Type, List<ulong>>>();
            behaviordict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, BehaviorInfo>>>();
            behaviorinfos = ObjectCache.Get<Dictionary<Type, List<BehaviorInfo>>>();
            actorassembleds = ObjectCache.Get<Dictionary<ulong, Actor>>();
            behaviorassembleds = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Behavior>>>();
        }

        protected override void OnReset()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            // 回收 Actor 列表
            actors.Clear();
            ObjectCache.Set(actors);
            
            // 回收 RmvActor 列表
            rmvactors.Clear();
            ObjectCache.Set(rmvactors);

            // 回收行为列表
            foreach (var kv in behaviors)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviors.Clear();
            ObjectCache.Set(behaviors);
            
            // 回收行为所有者列表
            foreach (var kv in behaviorowners)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviorowners.Clear();
            ObjectCache.Set(behaviorowners);
            
            // 回收行为信息集合
            foreach (var kv in behaviordict)
            {
                foreach (var kv2 in kv.Value)
                {
                    kv2.Value.Reset();
                    ObjectCache.Set(kv2.Value);
                }
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviordict.Clear();
            ObjectCache.Set(behaviordict);
            
            // 回收行为信息列表
            foreach (var kv in behaviorinfos)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviorinfos.Clear();
            ObjectCache.Set(behaviorinfos);
            
            // 回收已组装的 Actor 列表
            actorassembleds.Clear();
            ObjectCache.Set(actorassembleds);
            
            // 回收已组装的行为列表
            behaviorassembleds.Clear();
            ObjectCache.Set(behaviorassembleds);
        }
    }
}