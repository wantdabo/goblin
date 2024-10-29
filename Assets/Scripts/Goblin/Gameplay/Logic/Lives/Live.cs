using Goblin.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Lives
{
    /// <summary>
    /// Actor 诞生事件
    /// </summary>
    public struct LiveBornEvent : IEvent
    {
    }
    
    /// <summary>
    /// Actor 死亡事件
    /// </summary>
    public struct LiveDeadEvent : IEvent
    {
    }

    /// <summary>
    /// Actor 苏醒事件
    /// </summary>
    public struct LiveAwakenEvent : IEvent
    {
    }

    /// <summary>
    /// 生命周期
    /// </summary>
    public class Live : Behavior
    {
        /// <summary>
        /// 存活
        /// </summary>
        public bool alive { get; private set; }

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
            alive = true;
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_BORN());
            actor.stage.Live(actor.id);
            actor.eventor.Tell<LiveAwakenEvent>();
        }
        
        private void OnLiveDead(LiveDeadEvent e)
        {
            alive = false;
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_DEAD());
            actor.stage.Dead(actor.id);
        }
    }
}
