using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Skills.Bullets.Common
{
    /// <summary>
    /// 子弹翻译
    /// </summary>
    public class Translator : Translator<SkillBullet>
    {
        /// <summary>
        /// 子弹 ID
        /// </summary>
        private uint id { get; set; }
        /// <summary>
        /// 子弹状态
        /// </summary>
        private byte state { get; set; }
        /// <summary>
        /// 拥有者/ActorID
        /// </summary>
        private uint owner { get; set; }

        protected override void OnRIL()
        {
            if (id != behavior.id || state != behavior.state || owner != behavior.owner)
            {
                id = behavior.id;
                state = behavior.state;
                owner = behavior.owner;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_BULLET_INFO(behavior.id, behavior.state, behavior.owner));
            }
        }
    }
}
