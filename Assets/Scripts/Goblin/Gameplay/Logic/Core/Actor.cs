using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 实体
    /// </summary>
    public sealed class Actor
    {
        /// <summary>
        /// ID
        /// </summary>
        public ulong id { get; private set; }
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; private set; }
        
        public void Assembly(ulong id, Stage stage)
        {
            this.id = id;
            this.stage = stage;
        }
    }
}