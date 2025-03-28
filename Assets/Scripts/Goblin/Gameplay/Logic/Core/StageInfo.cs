using System;
using System.Collections.Generic;
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
        public FP timescale { get; set; } = FP.One;
        public ulong increment { get; set; } = 0;
        public Dictionary<Type, Translator> translators { get; set; } = new();
        public List<ulong> actors { get; set; } = new();
        public List<ulong> rmvactors { get; set; } = new();
        public Dictionary<ulong, List<Type>> behaviors { get; set; } = new();
        public Dictionary<Type, List<ulong>> behaviorowners { get; set; } = new();
        public Dictionary<ulong, Dictionary<Type, IBehaviorInfo>> behaviorinfos { get; set; } = new();
        public Dictionary<ulong, Actor> actorassembleds { get; set; } = new();
        public Dictionary<ulong, Dictionary<Type, Behavior>> behaviorassembleds { get; set; } = new();
        
        public void OnReady()
        {
        }

        public void OnReset()
        {
        }
    }
}