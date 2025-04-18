using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 代理
    /// </summary>
    public abstract class Agent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public ulong actor { get; private set; }
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        /// <summary>
        /// 抵达过了
        /// </summary>
        private bool arrived { get; set; }
        
        public void Ready(ulong id, World world)
        {
            this.actor = id;
            this.world = world;
            OnReady();
        }
        
        public void Reset()
        {
            OnReset();
            this.actor = 0;
            this.world = null;
            arrived = false;
        }
        
        public void Arrive()
        {
            arrived = true;
            OnArrive();
        }

        public void Chase()
        {
            if (false == arrived)
            {
                Arrive();
                
                return;
            }

            OnChase();
        }

        protected abstract void OnReady();
        protected abstract void OnReset();
        protected virtual void OnArrive() { }
        protected virtual void OnChase() { }
    }
}