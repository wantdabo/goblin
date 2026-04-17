using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.EVENT
{
    /// <summary>
    /// RIL 事件 - 伤害事件
    /// </summary>
    public class RIL_EVENT_DAMAGE : IRIL_EVENT
    {
        public override ushort id => RIL_DEFINE.EVENT_DAMAGE;
        
        /// <summary>
        /// 来源, ActorID
        /// </summary>
        public ulong from { get; set; }
        /// <summary>
        /// 去向, ActorID
        /// </summary>
        public ulong to { get; set; }
        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool crit { get; set; }
        /// <summary>
        /// 伤害值
        /// </summary>
        public int damage { get; set; }

        protected override void OnReset()
        {
            from = 0;
            to = 0;
            crit = false;
            damage = 0;
        }

        protected override void OnClone(IRIL_EVENT clone)
        {
            if (clone is not RIL_EVENT_DAMAGE e) return;
            
            e.from = from;
            e.to = to;
            e.crit = crit;
            e.damage = damage;
        }
    }
}