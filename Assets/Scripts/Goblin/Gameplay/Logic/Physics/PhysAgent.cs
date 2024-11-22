using Goblin.Common;
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
        /// 默认物理材质
        /// </summary>
        private static Material defaultMaterial = new(FP.Zero, FP.Zero);

        /// <summary>
        /// 物理单位
        /// </summary>
        public Rigidbody rigidbody { get; private set; }

        private Spatial spatial { get; set; }

        private FPVector3 mrigidbodyoffset = FPVector3.zero;
        /// <summary>
        /// 物理单位平移偏移
        /// </summary>
        public FPVector3 rigidbodyoffset
        {
            get
            {
                return mrigidbodyoffset;
            }
            set
            {
                mrigidbodyoffset = value;
                rigidbody.position = rigidbodyoffset + spatial.position;
            }
        }

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

        /// <summary>
        /// 圆柱体
        /// </summary>
        public CylinderShape cylindershape
        {
            get
            {
                var shape = rigidbody.shape as CylinderShape;
                if (null == shape)
                {
                    shape = new CylinderShape(FPVector3.zero, FP.One, FP.One);
                    rigidbody.shape = shape;
                }

                return shape;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = actor.GetBehavior<Spatial>();
            rigidbody = actor.stage.phys.AddBody(actor.id, new BoxShape(FPVector3.zero, FPVector3.one), FP.One, defaultMaterial);
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
            actor.eventor.UnListen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.UnListen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            rigidbody.CollisionEnter -= OnCollisionEnter;
            rigidbody.CollisionExit -= OnCollisionExit;
            rigidbody.TriggerEnter -= OnTriggerEnter;
            rigidbody.TriggerExit -= OnTriggerExit;
            actor.stage.phys.RmvBody(actor.id);
        }

        private void OnSpatialPositionChanged(SpatialPositionChangedEvent e)
        {
            rigidbody.position = rigidbodyoffset + e.position;
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
            UnityEngine.Debug.Log("OnTriggerEnter");
            actor.eventor.Tell(new CollisionEnterEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }

        private void OnTriggerExit(Collider collider)
        {
            UnityEngine.Debug.Log("OnTriggerExit");
            actor.eventor.Tell(new CollisionExitEvent { actorId = actor.stage.phys.GetActorId(collider.rigidbody) });
        }
    }
}
