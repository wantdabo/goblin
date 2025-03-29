using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 代理
    /// </summary>
    public sealed class Agent : Comp
    {
        /// <summary>
        /// ID
        /// </summary>
        public ulong id { get; private set; }
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Behavior 集合
        /// </summary>
        private Dictionary<Type, Talent> talentdict { get; set; }
    }
}