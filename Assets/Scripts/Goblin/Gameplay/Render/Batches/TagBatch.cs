using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
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
        protected override void OnTick(TickEvent e)
        {
            if (false == world.statebucket.GetStates<TagState>(StateType.Tag, out var states)) return;
            foreach (var state in states)
            {
                if (false == state.tags.TryGetValue(TAG_DEFINE.MODEL_ID, out var m))continue;
                if (0 != m)
                {
                    var model = world.EnsureAgent<ModelAgent>(state.actor);
                    model.Load(m);
                }
            }
            states.Clear();
            ObjectCache.Set(states);
        }
    }
}