using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    public abstract class Resolver : Comp
    {
        public World world { get; private set; }
        
        public Resolver Initialize(World world)
        {
            this.world = world;

            return this;
        }
    }
}