using Goblin.Core;
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
        public abstract ushort id { get; }

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
            if (id != e.rilstate.ril.id) return;
            var state = OnRIL(e.rilstate, (T)e.rilstate.ril);
            if (RIL_DEFINE.TYPE_EVENT == e.rilstate.type) return;
            state.actor = e.rilstate.actor;
            state.frame = e.rilstate.frame;
            statebucket.SetState(state);
        }

        protected abstract IState OnRIL(RILState rilstate, T ril);
    }
}