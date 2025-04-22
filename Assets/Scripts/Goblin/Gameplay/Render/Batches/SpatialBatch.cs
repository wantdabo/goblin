using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Batches
{
    public class SpatialBatch : Batch
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
            var spatialbundles = world.statebucket.GetStates(RIL_DEFINE.SPATIAL);
            if (null == spatialbundles) return;
            foreach (var bundle in spatialbundles)
            {
                var spatial = (RIL_SPATIAL)bundle.ril;
                var node = world.EnsureAgent<NodeAgent>(bundle.actor);
                node.targetPosition = spatial.position.ToVector3();
                node.targetEuler = spatial.euler.ToVector3();
                node.targetScale = spatial.scale.ToVector3();
                node.Chase();
            }
        }
    }
}