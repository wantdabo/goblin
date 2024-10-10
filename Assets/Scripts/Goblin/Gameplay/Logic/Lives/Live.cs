using Goblin.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Lives
{
    public struct LiveBornEvent : IEvent
    {
    }
    
    public struct LiveDeadEvent : IEvent
    {
    }

    public struct LiveAwakenEvent : IEvent
    {
        
    }

    public class Live : Behavior
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<LiveBornEvent>(OnLiveBorn);
            actor.eventor.Listen<LiveDeadEvent>(OnLiveDead);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<LiveBornEvent>(OnLiveBorn);
            actor.eventor.UnListen<LiveDeadEvent>(OnLiveDead);
        }
        
        private void OnLiveBorn(LiveBornEvent e)
        {
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_BORN());
            actor.eventor.Tell<LiveAwakenEvent>();
        }
        
        private void OnLiveDead(LiveDeadEvent e)
        {
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_DEAD());
        }
    }
}
