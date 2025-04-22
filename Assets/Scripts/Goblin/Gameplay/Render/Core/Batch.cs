using Goblin.Common;
using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    public abstract class Batch : Comp
    {
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