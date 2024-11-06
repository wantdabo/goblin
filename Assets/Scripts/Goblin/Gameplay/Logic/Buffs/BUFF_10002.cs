using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Buffs.Common;
using Goblin.Gameplay.Logic.Skills;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Buffs
{
    /// <summary>
    /// 引爆感电 BUFF
    /// </summary>
    public class BUFF_10002 : Buff
    {
        public override uint id => BUFF_DEFINE.BUFF_10002;

        public override byte type => BUFF_DEFINE.SHARED;

        public override uint maxlayer => 1;

        private List<uint> actorIds { get; set; } = new();

        protected override bool OnValid()
        {
            return layer >= 1;
        }

        protected override void OnTrigger()
        {
            base.OnTrigger();
            
            if (0 == actorIds.Count) return;

            // TODO 后面修改为读取配置, 使用配置中的技能伤害强度
            var hurt = new HurtEvent()
            {
                damage = bucket.actor.stage.calc.ChargeDamage(bucket.actor.id, 1),
                from = bucket.actor.id
            };

            foreach (uint actorId in actorIds)
            {
                var target = bucket.actor.stage.GetActor(actorId);
                if (null == target || false == target.live.alive) continue;
                target.eventor.Tell(new BuffEraseEvent() { id = BUFF_DEFINE.BUFF_10001, layer = uint.MaxValue, from = bucket.actor.id });
                target.eventor.Tell(hurt);
            }

            Erase(uint.MaxValue);
        }

        protected override void OnStamp()
        {
            base.OnStamp();
            actorIds.Clear();
            bucket.actor.eventor.Listen<SkillCollisionEvent>(OnSkillCollision);
        }

        protected override void OnErase()
        {
            base.OnErase();
            actorIds.Clear();
            bucket.actor.eventor.UnListen<SkillCollisionEvent>(OnSkillCollision);
        }

        private void OnSkillCollision(SkillCollisionEvent e)
        {
            for (int i = 0; i < e.actorIds.Length; i++)
            {
                var actorId = e.actorIds[i];
                if (actorIds.Contains(actorId)) continue;
                actorIds.Add(actorId);
            }
        }
    }
}
