using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;

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
            var tagbundles = world.statebucket.GetStates(RIL_DEFINE.TAG);
            if (null == tagbundles) return;
            foreach (var bundle in tagbundles)
            {
                var tag = (RIL_TAG) bundle.ril;
                if (0 != tag.model)
                {
                    var model = world.EnsureAgent<ModelAgent>(bundle.actor);
                    model.Load(tag.model);
                }
            }
        }
    }
}