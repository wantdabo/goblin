using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Lives;
using Goblin.Gameplay.Logic.Physics;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Bullets.Common
{
    /// <summary>
    /// 子弹发射事件
    /// </summary>
    public struct SkillBulletFireEvent : IEvent
    {
        /// <summary>
        /// 拥有者/ActorID
        /// </summary>
        public uint owner { get; set; }
        /// <summary>
        /// 起始位置
        /// </summary>
        public TSVector position { get; set; }
        /// <summary>
        /// 伤害
        /// </summary>
        public DamageInfo damage { get; set; }
        /// <summary>
        /// 传参
        /// </summary>
        public object[] args { get; set; }
    }

    /// <summary>
    /// 子弹停止事件
    /// </summary>
    public struct SkillBulletStopEvent : IEvent
    {
    }

    /// <summary>
    /// 子弹行为
    /// </summary>
    public abstract class SkillBullet : Behavior<Translator>
    {
        /// <summary>
        /// 子弹 ID
        /// </summary>
        public abstract uint id { get; }
        /// <summary>
        /// 子弹状态
        /// </summary>
        public byte state { get; private set; } = SKILL_BULLET_DEFINE.NONE;
        /// <summary>
        /// 拥有者/ActorID
        /// </summary>
        public uint owner { get; private set; }
        /// <summary>
        /// 起始位置
        /// </summary>
        public TSVector position { get; private set; }
        /// <summary>
        /// 伤害
        /// </summary>
        public DamageInfo damage { get; private set; }
        /// <summary>
        /// 传参
        /// </summary>
        protected object[] args { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<SkillBulletFireEvent>(OnSkillBulletFire);
            actor.eventor.Listen<SkillBulletStopEvent>(OnSkillBulletStop);
            actor.eventor.Listen<ActorDeadEvent>(OnActorDead);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<SkillBulletFireEvent>(OnSkillBulletFire);
            actor.eventor.UnListen<SkillBulletStopEvent>(OnSkillBulletStop);
            actor.eventor.Listen<ActorDeadEvent>(OnActorDead);
        }

        private void OnFPTick(FPTickEvent e)
        {
            state = SKILL_BULLET_DEFINE.FLYING;
            OnFlying(e.tick);
            translator.Force();
        }

        private void OnSkillBulletFire(SkillBulletFireEvent e)
        {
            owner = e.owner;
            position = e.position;
            damage = e.damage;
            args = e.args;

            actor.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            actor.eventor.Listen<CollisionEnterEvent>(OnCollisionEnter);
            actor.eventor.Listen<CollisionExitEvent>(OnCollisionExit);

            actor.live.Born();
            state = SKILL_BULLET_DEFINE.FIRE;
            OnFire();
            translator.Force();
        }

        private void OnSkillBulletStop(SkillBulletStopEvent e)
        {
            actor.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            actor.eventor.UnListen<CollisionEnterEvent>(OnCollisionEnter);
            actor.eventor.UnListen<CollisionExitEvent>(OnCollisionExit);

            state = SKILL_BULLET_DEFINE.STOP;
            OnStop();
            translator.Force();
            actor.live.Dead();
        }

        private void OnActorDead(ActorDeadEvent e)
        {
            if (e.actor.id != owner) return;
            OnOwnerDead();
        }

        private void OnCollisionEnter(CollisionEnterEvent e)
        {
            if (e.actorId == owner) return;

            var target = actor.stage.GetActor(e.actorId);
            if (null == target || false == target.live.alive) return;
            OnEnter(target);
        }

        private void OnCollisionExit(CollisionExitEvent e)
        {
            if (e.actorId == owner) return;

            var target = actor.stage.GetActor(e.actorId);
            if (null == target || false == target.live.alive) return;
            OnExit(target);
        }

        /// <summary>
        /// 开火
        /// </summary>
        protected virtual void OnFire() { }
        /// <summary>
        /// 停止
        /// </summary>
        protected virtual void OnStop() { }
        /// <summary>
        /// 飞行
        /// </summary>
        /// <param name="tick">tick</param>
        protected virtual void OnFlying(FP tick) { }
        /// <summary>
        /// 拥有者死亡
        /// </summary>
        protected virtual void OnOwnerDead() { }
        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="target">目标/Actor</param>
        protected virtual void OnEnter(Actor target) { }
        /// <summary>
        /// 碰撞退出
        /// </summary>
        /// <param name="target">目标/Actor</param>
        protected virtual void OnExit(Actor target) { }
    }
}
