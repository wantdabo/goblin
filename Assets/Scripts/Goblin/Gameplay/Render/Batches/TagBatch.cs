using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Batches
{
    public class TagBatch : Batch
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        private void OnTick(TickEvent e)
        {
            var states = world.statebucket.GetStates<TagState>(StateType.Tag);
            if (null == states) return;
            foreach (var state in states)
            {
                if (0 != state.model)
                {
                    var model = world.EnsureAgent<ModelAgent>(state.actor);
                    model.Load(state.model);
                }
            }
        }
    }
}