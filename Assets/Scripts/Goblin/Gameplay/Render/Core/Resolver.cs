using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    public abstract class Resolver : Comp
    {
        public World world { get; private set; }
    }
}