using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa
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
    
    /// <summary>
    /// 场景信息
    /// </summary>
    public class StageInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前 Stage 状态
        /// </summary>
        public StageState state { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
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
        
        protected override void OnReady()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            timescale = FP.One;
            increment = 0;
            
            actors = ObjectCache.Ensure<List<ulong>>();
            behaviortypes = ObjectCache.Ensure<Dictionary<ulong, List<Type>>>();
            behaviorinfos = ObjectCache.Ensure<Dictionary<Type, List<BehaviorInfo>>>();
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
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<StageInfo>();
            clone.Ready(actor);
            clone.state = state;
            clone.frame = frame;
            clone.elapsed = elapsed;
            clone.timescale = timescale;
            clone.increment = increment;

            clone.actors.AddRange(actors);
            
            foreach (var kv in behaviortypes)
            {
                var list = ObjectCache.Ensure<List<Type>>();
                foreach (var type in kv.Value)
                {
                    list.Add(type);
                }
                clone.behaviortypes.Add(kv.Key, list);
            }
            
            foreach (var kv in behaviorinfos)
            {
                var list = ObjectCache.Ensure<List<BehaviorInfo>>();
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