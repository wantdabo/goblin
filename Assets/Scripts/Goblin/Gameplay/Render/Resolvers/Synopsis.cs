using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public struct SynopsisEvent : IEvent
    {
        public RIL_SYNOPSIS synopsis { get; set; }
    }

    public class Synopsis : Resolver
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
            var synopsisbundles = world.summary.GetStateBundles(RIL_DEFINE.SYNOPSIS);
            if (null == synopsisbundles) return;
            foreach (var bundle in synopsisbundles)
            {
                var synopsis = (RIL_SYNOPSIS)bundle.ril;
                world.engine.proxy.gameplay.eventor.Tell(new SynopsisEvent
                {
                    synopsis = synopsis,
                });
            }
            
            var seatbundles = world.summary.GetStateBundles(RIL_DEFINE.SEAT);
            if (null == seatbundles) return;
            foreach (var bundle in seatbundles)
            {
                var seat = (RIL_SEAT)bundle.ril;
                UnityEngine.Debug.Log(seat.ToString());
            }
        }
    }
}