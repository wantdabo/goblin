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

    public abstract class Resolver<T> : Resolver where T : IRIL
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
            
            var state = OnRIL(e.rilstate, rilstate.ril);
            if (RIL_DEFINE.TYPE_RENDER == e.rilstate.type)
            {
                state.actor = e.rilstate.actor;
                state.frame = e.rilstate.frame;
                statebucket.SetState(state);
            }
            
            rilstate.Reset();
            ObjectCache.Set(rilstate);
        }

        protected abstract IState OnRIL(RILState rilstate, T ril);
    }
}