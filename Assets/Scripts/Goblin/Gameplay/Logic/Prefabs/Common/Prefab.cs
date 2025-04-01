using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Prefabs.Common
{
    public interface IPrefabInfo
    {
        
    }
    
    public abstract class Prefab
    {
        protected Stage stage { get; private set; }

        public Prefab Initialize(Stage stage)
        {
            this.stage = this.stage;

            return this;
        }

        public Actor Processing(Actor actor, IPrefabInfo info)
        {
            return OnProcessing(actor, info);
        }

        protected virtual Actor OnProcessing(Actor actor, IPrefabInfo info)
        {
            return default;
        }
    }
    
    public abstract class Prefab<T> : Prefab where T : IPrefabInfo
    {
        public abstract byte type { get; }

        protected override Actor OnProcessing(Actor actor, IPrefabInfo info)
        {
            if (actor.SeekBehavior(out Tag tag))
            {
                tag.Set(TAG_DEFINE.ACTOR_TYPE, type);
            }
            
            OnProcessing(actor, (T)info);

            return actor;
        }

        public abstract void OnProcessing(Actor actor, T info);
    }
}