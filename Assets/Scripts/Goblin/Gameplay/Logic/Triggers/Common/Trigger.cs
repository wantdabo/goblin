using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Triggers.Common
{
    public sealed class Trigger
    {
        private Actor actor { get; set; }
        
        public void Load(Actor actor)
        {
            this.actor = actor;
        }
        
        public void Unload()
        {
            this.actor = null;
        }
    }
}