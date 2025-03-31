using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
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
    
    public class StageInfo : IBehaviorInfo
    {
        public StageState state { get; set; } = StageState.None;
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        public FP tick => GAME_DEFINE.LOGIC_TICK * timescale;
        public FP timescale { get; set; } = FP.One;
        public ulong increment { get; set; } = 0;
        
        public List<ulong> actors { get; set; }
        public List<ulong> rmvactors { get; set; }
        public Dictionary<ulong, List<Type>> behaviors { get; set; }
        public Dictionary<Type, List<ulong>> behaviorowners { get; set; }
        public Dictionary<ulong, Dictionary<Type, IBehaviorInfo>> behaviorinfos { get; set; }
        public Dictionary<ulong, Actor> actorassembleds { get; set; }
        public Dictionary<ulong, Dictionary<Type, Behavior>> behaviorassembleds { get; set; }
        
        public void Ready()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            increment = 0;
            increment = 0;
            
            actors = ObjectCache.Get<List<ulong>>();
            rmvactors = ObjectCache.Get<List<ulong>>();
            behaviors = ObjectCache.Get<Dictionary<ulong, List<Type>>>();
            behaviorowners = ObjectCache.Get<Dictionary<Type, List<ulong>>>();
            behaviorinfos = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, IBehaviorInfo>>>();
            actorassembleds = ObjectCache.Get<Dictionary<ulong, Actor>>();
            behaviorassembleds = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Behavior>>>();
        }

        public void Reset()
        {
            state = StageState.None;
            frame = 0;
            elapsed = 0;
            increment = 0;
            increment = 0;
            
            actors.Clear();
            ObjectCache.Set(actors);
            
            rmvactors.Clear();
            ObjectCache.Set(rmvactors);

            foreach (var kv in behaviors)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviors.Clear();
            ObjectCache.Set(behaviors);
            
            foreach (var kv in behaviorowners)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviorowners.Clear();
            ObjectCache.Set(behaviorowners);
            
            foreach (var kv in behaviorinfos)
            {
                foreach (var kv2 in kv.Value)
                {
                    kv2.Value.Reset();
                    ObjectCache.Set(kv2.Value);
                }
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviorinfos.Clear();
            ObjectCache.Set(behaviorinfos);
            
            actorassembleds.Clear();
            ObjectCache.Set(actorassembleds);
            
            behaviorassembleds.Clear();
            ObjectCache.Set(behaviorassembleds);
        }
    }
}