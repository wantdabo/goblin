using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Batches
{
    public struct StageEvent : IEvent
    {
        public StageState state { get; set; }
    }

    public class StageBatch : Batch
    {
        protected override void OnTick(TickEvent e)
        {
            if (false == world.statebucket.GetStates<StageState>(StateType.Stage, out var states)) return;
            foreach (var state in states)
            {
                world.engine.proxy.gameplay.eventor.Tell(new StageEvent
                {
                    state = state,
                });
            }
            
            states.Clear();
            ObjectCache.Set(states);
        }
    }
}