using Goblin.Common;
using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 批处理
    /// </summary>
    public abstract class Batch : Comp
    {
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            world.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            world.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
        }

        /// <summary>
        /// 初始化批处理
        /// </summary>
        /// <param name="world">世界</param>
        /// <returns>批处理</returns>
        public Batch Initialize(World world)
        {
            this.world = world;

            return this;
        }
        
        protected virtual void OnTick(TickEvent e)
        {
        }

        protected virtual void OnLateTick(LateTickEvent e)
        {
        }
    }
}