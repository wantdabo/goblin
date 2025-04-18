using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public struct StageEvent : IEvent
    {
        public RIL_STAGE stage { get; set; }
    }

    public class Stage : Resolver
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
            var synopsisbundles = world.statebucket.GetStateBundles(RIL_DEFINE.STAGE);
            if (null == synopsisbundles) return;
            foreach (var bundle in synopsisbundles)
            {
                var synopsis = (RIL_STAGE)bundle.ril;
                world.engine.proxy.gameplay.eventor.Tell(new StageEvent
                {
                    stage = synopsis,
                });
            }
        }
    }
}