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
        /// 行为类型列表, 键为 ActorID, 值为该 Actor 上的所有行为类型
        /// </summary>
        public Dictionary<ulong, List<Type>> behaviortypes { get; set; }
        /// <summary>
        /// 行为信息列表, 键为行为类型, 值为该行为类型的所有 BehaviorInfo 列表
        /// </summary>
        public Dictionary<Type, List<BehaviorInfo>> behaviorinfos { get; set; }
        
        /// <summary>
        /// Actor 列表, 键为 ActorID, 值为 Actor 实例
        /// </summary>
        public Dictionary<ulong, Actor> actordict { get; set; }
        /// <summary>
        /// 行为列表, 键为行为类型, 值为该行为类型的所有 Behavior 列表
        /// </summary>
        public Dictionary<Type, List<Behavior>> behaviors { get; set; }
        /// <summary>
        /// 行为列表, 键为 ActorID, 值为该 Actor 上的所有行为
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, Behavior>> behaviordict { get; set; }
        /// <summary>
        /// 行为信息列表, 键为 ActorID, 值为该 Actor 上的所有行为信息
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfodict { get; set; }
        /// <summary>
        /// RmvActor 列表
        /// </summary>
        public List<ulong> rmvactors { get; set; }
        
        protected override void OnReady()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            actors = ObjectCache.Get<List<ulong>>();
            behaviortypes = ObjectCache.Get<Dictionary<ulong, List<Type>>>();
            behaviorinfos = ObjectCache.Get<Dictionary<Type, List<BehaviorInfo>>>();
            actordict = ObjectCache.Get<Dictionary<ulong, Actor>>();
            behaviors = ObjectCache.Get<Dictionary<Type, List<Behavior>>>();
            behaviordict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Behavior>>>();
            behaviorinfodict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, BehaviorInfo>>>();
            rmvactors = ObjectCache.Get<List<ulong>>();

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
            
            behaviortypes.Clear();
            ObjectCache.Set(behaviortypes);
            
            behaviorinfos.Clear();
            ObjectCache.Set(behaviorinfos);
            
            actordict.Clear();
            ObjectCache.Set(actordict);
            
            behaviors.Clear();
            ObjectCache.Set(behaviors);
            
            behaviordict.Clear();
            ObjectCache.Set(behaviordict);
            
            behaviorinfodict.Clear();
            ObjectCache.Set(behaviorinfodict);
            
            rmvactors.Clear();
            ObjectCache.Set(rmvactors);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<StageInfo>();
            clone.Ready(id);
            clone.state = state;
            clone.frame = frame;
            clone.elapsed = elapsed;
            clone.timescale = timescale;
            clone.increment = increment;

            foreach (var actor in actors)
            {
                clone.actors.Add(actor);
            }
            
            foreach (var kv in behaviortypes)
            {
                var list = ObjectCache.Get<List<Type>>();
                foreach (var type in kv.Value)
                {
                    list.Add(type);
                }
                clone.behaviortypes.Add(kv.Key, list);
            }
            
            foreach (var kv in behaviorinfos)
            {
                var list = ObjectCache.Get<List<BehaviorInfo>>();
                foreach (var info in kv.Value)
                {
                    list.Add(info.Clone());
                }
                clone.behaviorinfos.Add(kv.Key, list);
            }

            return clone;
        }
    }
}