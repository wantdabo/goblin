using Goblin.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Lives
{
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

        /// <summary>
        /// 出生
        /// </summary>
        public void Born()
        {
            alive = true;
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_BORN());
            actor.stage.Live(actor.id);
            actor.eventor.Tell<LiveAwakenEvent>();
        }
        
        /// <summary>
        /// 死亡
        /// </summary>
        public void Dead()
        {
            alive = false;
            actor.stage.rilsync.PushRIL(actor.id, new RIL_LIVE_DEAD());
            actor.stage.Dead(actor.id);
        }
    }
}
