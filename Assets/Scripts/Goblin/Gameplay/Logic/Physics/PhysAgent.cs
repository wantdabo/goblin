using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Logic.Spatials;
using Kowtow;
using Kowtow.Collision;
using Kowtow.Collision.Shapes;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Physics
{
    /// <summary>
    /// 碰撞进入事件
    /// </summary>
    public struct CollisionEnterEvent : IEvent
    {
        /// <summary>
        /// ActorID
        /// </summary>
        public uint actorId { get; set; }
    }

    /// <summary>
    /// 碰撞退出事件
    /// </summary>
    public struct CollisionExitEvent : IEvent
    {
        /// <summary>
        /// ActorID
        /// </summary>
        public uint actorId { get; set; }
    }

    /// <summary>
    /// 物理代理
    /// </summary>
    public class PhysAgent : Behavior
    {
        /// <summary>
        /// 物理单位
        /// </summary>
        public Rigidbody rigidbody { get; private set; }

        /// <summary>
        /// 是否在地面上
        /// </summary>
        public bool grounded
        {
            get
            {
                // 计算盒子底部中心点
                var bottomCenter = (FPVector3.down * FP.EN3) + rigidbody.aabb.position + new FPVector3(FP.Zero, -rigidbody.aabb.size.y * FP.Half, FP.Zero);
                // 计算底部的四个角点
                FPVector3 bottomFrontLeft = bottomCenter + new FPVector3(-rigidbody.aabb.size.x * FP.Half, FP.Zero, -rigidbody.aabb.size.z * FP.Half);
                FPVector3 bottomFrontRight = bottomCenter + new FPVector3(rigidbody.aabb.size.x * FP.Half, FP.Zero, -rigidbody.aabb.size.z * FP.Half);
                FPVector3 bottomBackLeft = bottomCenter + new FPVector3(-rigidbody.aabb.size.x * FP.Half, FP.Zero, rigidbody.aabb.size.z * FP.Half);
                FPVector3 bottomBackRight = bottomCenter + new FPVector3(rigidbody.aabb.size.x * FP.Half, FP.Zero, rigidbody.aabb.size.z * FP.Half);

                var result = actor.stage.phys.Linecast(rigidbody.aabb.position, bottomCenter, false, Layer.Ground);
                if (result.hit) return true;
                result = actor.stage.phys.Linecast(rigidbody.aabb.position, bottomFrontLeft, false, Layer.Ground);
                if (result.hit) return true;
                result = actor.stage.phys.Linecast(rigidbody.aabb.position, bottomFrontRight, false, Layer.Ground);
                if (result.hit) return true;
                result = actor.stage.phys.Linecast(rigidbody.aabb.position, bottomBackLeft, false, Layer.Ground);
                if (result.hit) return true;
                result = actor.stage.phys.Linecast(rigidbody.aabb.position, bottomBackRight, false, Layer.Ground);
                if (result.hit) return true;

                return false;
            }
        }

        private Spatial spatial { get; set; }

        /// <summary>
        /// 立方体
        /// </summary>
        public BoxShape boxshape
        {
            get
            {
                var shape = rigidbody.shape as BoxShape;
                if (null == shape)
                {
                    shape = new BoxShape(FPVector3.zero, FPVector3.one);
                    rigidbody.shape = shape;
                }

                return shape;
            }
        }

        /// <summary>
        /// 球体
        /// </summary>
        public SphereShape sphereshape
        {
            get
            {
                var shape = rigidbody.shape as SphereShape;
                if (null == shape)
                {
                    shape = new SphereShape(FPVector3.zero, FP.One);
                    rigidbody.shape = shape;
                }

                return shape;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = actor.GetBehavior<Spatial>();
            rigidbody = actor.stage.phys.AddBody(actor.id, new BoxShape(FPVector3.zero, FPVector3.one), FP.One);
            actor.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
            actor.eventor.Listen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.Listen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            rigidbody.CollisionEnter += OnCollisionEnter;
            rigidbody.CollisionExit += OnCollisionExit;
            rigidbody.TriggerEnter += OnTriggerEnter;
            rigidbody.TriggerExit += OnTriggerExit;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
            actor.eventor.UnListen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.UnListen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            rigidbody.CollisionEnter -= OnCollisionEnter;
            rigidbody.CollisionExit -= OnCollisionExit;
            rigidbody.TriggerEnter -= OnTriggerEnter;
            rigidbody.TriggerExit -= OnTriggerExit;
            actor.stage.phys.RmvBody(actor.id);
        }

        /// <summary>
        /// 失去所有力 & 速度
        /// </summary>
        public void LossForce()
        {
            rigidbody.force = FPVector3.zero;
            rigidbody.velocity = FPVector3.zero;
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            spatial.position = rigidbody.position;
            spatial.rotation = rigidbody.rotation;
        }

        private void OnSpatialPositionChanged(SpatialPositionChangedEvent e)
        {
            rigidbody.position = e.position;
        }

        private void OnSpatialRotationChanged(SpatialRotationChangedEvent e)
        {
            rigidbody.rotation = e.rotation;
        }

        private void OnCollisionEnter(Collider collider)
        {
            actor.eventor.Tell(new CollisionEnterEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }

        private void OnCollisionExit(Collider collider)
        {
            actor.eventor.Tell(new CollisionExitEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }

        private void OnTriggerEnter(Collider collider)
        {
            actor.eventor.Tell(new CollisionEnterEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }

        private void OnTriggerExit(Collider collider)
        {
            actor.eventor.Tell(new CollisionExitEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }
    }
}
