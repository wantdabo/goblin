using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class Spatial : Resolver
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
            var spatialbundles = world.summary.GetStateBundles(RIL_DEFINE.SPATIAL);
            if (null == spatialbundles) return;
            foreach (var bundle in spatialbundles)
            {
                var node = world.EnsureAgent<Node>(bundle.actor);
                var spatial = (RIL_SPATIAL)bundle.ril;
                node.go.transform.position = spatial.position.ToVector3();
                node.go.transform.rotation = Quaternion.Euler(spatial.euler.ToVector3());
                node.go.transform.localScale = spatial.scale.ToVector3();
            }
        }
    }
}