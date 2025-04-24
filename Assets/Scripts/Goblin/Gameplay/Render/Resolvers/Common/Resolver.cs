using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    public abstract class Resolver : Comp
    {
        public StateBucket statebucket { get; private set; }

        public Resolver Initialize(StateBucket statebucket)
        {
            this.statebucket = statebucket;

            return this;
        }
    }

    public abstract class Resolver<T, E> : Resolver where T : IRIL where E : State
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            statebucket.world.eventor.Listen<RILEvent>(OnRIL);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            statebucket.world.eventor.UnListen<RILEvent>(OnRIL);
        }
        
        private void OnRIL(RILEvent e)
        {
            if (e.rilstate is not RILState<T> rilstate) return;
            E state = OnRIL(rilstate);
            state.actor = rilstate.actor;
            state.hashcode = rilstate.ril.GetHashCode();
            statebucket.SetState(rilstate.type, state);
            
            rilstate.Reset();
            ObjectCache.Set(rilstate);
        }

        protected abstract E OnRIL(RILState<T> rilstate);
    }
}