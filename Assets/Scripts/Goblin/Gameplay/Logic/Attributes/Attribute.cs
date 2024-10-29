using Goblin.Common;
using Goblin.Gameplay.Logic.Attributes.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Skills;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Attributes
{
    /// <summary>
    /// 治疗事件
    /// </summary>
    public struct CureEvent : IEvent
    {
        /// <summary>
        /// 治疗数值
        /// </summary>
        public uint cure;
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from;
    }
    
    /// <summary>
    /// 治疗事件（已经过受方计算）
    /// </summary>
    public struct RecvCureEvent : IEvent
    {
        /// <summary>
        /// 治疗数值
        /// </summary>
        public uint cure;
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from;
    }

    /// <summary>
    /// 伤害事件
    /// </summary>
    public struct HurtEvent : IEvent
    {
        /// <summary>
        /// 伤害
        /// </summary>
        public DamageInfo damage { get; set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from { get; set; }
    }
    
    /// <summary>
    /// 伤害事件（已经过受方计算）
    /// </summary>
    public struct RecvHurtEvent : IEvent
    {
        /// <summary>
        /// 伤害
        /// </summary>
        public DamageInfo damage;
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from;
    }

    /// <summary>
    /// 属性
    /// </summary>
    public class Attribute : Behavior<Translator>
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public uint hp { get; set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public FP movespeed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<CureEvent>(OnCure);
            actor.eventor.Listen<HurtEvent>(OnHurt);
            actor.eventor.Listen<SkillCollisionEvent>(OnSkillCollision);
            actor.stage.calc.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<CureEvent>(OnCure);
            actor.eventor.UnListen<HurtEvent>(OnHurt);
            actor.eventor.UnListen<SkillCollisionEvent>(OnSkillCollision);
            actor.stage.calc.UnRegister(this);
        }

        private void OnCure(CureEvent e)
        {
            hp = Math.Clamp(hp + e.cure, 0, maxhp);
            actor.eventor.Tell(new RecvCureEvent()
            {
                cure = e.cure,
                from = e.from,
            });
        }

        private void OnHurt(HurtEvent e)
        {
            var damage = actor.stage.calc.DischargeDamage(actor.id, e.damage);
            // TODO 后面修改使用 damage 的数值
            damage.value = (uint)actor.stage.random.Range(100, 1000);
            hp = Math.Clamp(hp - damage.value, 0, maxhp);
            actor.eventor.Tell(new RecvHurtEvent()
            {
                damage = damage,
                from = e.from,
            });
        }

        private void OnSkillCollision(SkillCollisionEvent e)
        {
            if (null == e.actorIds || 0 == e.actorIds.Length) return;

            var hurt = new HurtEvent()
            {
                // TODO 后面修改为读取配置, 使用配置中的技能伤害强度
                damage = actor.stage.calc.ChargeDamage(actor.id, 1),
                from = actor.id
            };

            foreach (uint actorId in e.actorIds)
            {
                var target = actor.stage.GetActor(actorId);
                if (null == target) continue;

                target.eventor.Tell(hurt);
            }
        }
    }
}
