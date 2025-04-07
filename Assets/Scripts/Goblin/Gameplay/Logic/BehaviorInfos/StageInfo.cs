using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.Translators.Common;
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
        /// Actor 列表, 键为 ActorID, 值为 Actor 实例
        /// </summary>
        public Dictionary<ulong, Actor> actordict { get; set; }
        /// <summary>
        /// RmvActor 列表
        /// </summary>
        public List<ulong> rmvactors { get; set; }
        /// <summary>
        /// 行为列表, 键为行为类型, 值为该行为类型的所有 Behavior 列表
        /// </summary>
        public Dictionary<Type, List<Behavior>> behaviors { get; set; }
        /// <summary>
        /// 行为列表, 键为 ActorID, 值为该 Actor 上的所有行为
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, Behavior>> behaviordict { get; set; }
        /// <summary>
        /// 行为信息列表, 键为行为类型, 值为该行为类型的所有 BehaviorInfo 列表
        /// </summary>
        public Dictionary<Type, List<BehaviorInfo>> behaviorinfos { get; set; }
        /// <summary>
        /// 行为信息列表, 键为 ActorID, 值为该 Actor 上的所有行为信息
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfodict { get; set; }
        
        protected override void OnReady()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            actors = ObjectCache.Get<List<ulong>>();
            actordict = ObjectCache.Get<Dictionary<ulong, Actor>>();
            rmvactors = ObjectCache.Get<List<ulong>>();
            behaviors = ObjectCache.Get<Dictionary<Type, List<Behavior>>>();
            behaviordict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Behavior>>>();
            behaviorinfos = ObjectCache.Get<Dictionary<Type, List<BehaviorInfo>>>();
            behaviorinfodict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, BehaviorInfo>>>();
        }

        protected override void OnReset()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            actors.Clear();
            ObjectCache.Set(actors);
            
            actordict.Clear();
            ObjectCache.Set(actordict);
            
            rmvactors.Clear();
            ObjectCache.Set(rmvactors);
            
            behaviors.Clear();
            ObjectCache.Set(behaviors);
            
            behaviordict.Clear();
            ObjectCache.Set(behaviordict);
            
            behaviorinfos.Clear();
            ObjectCache.Set(behaviorinfos);
            
            behaviorinfodict.Clear();
            ObjectCache.Set(behaviorinfodict);
        }
    }
}