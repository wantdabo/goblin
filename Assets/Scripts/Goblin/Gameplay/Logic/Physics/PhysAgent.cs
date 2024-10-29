using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;
using TrueSync.Physics3D;

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
        private static BodyMaterial defaultMaterial = new(FP.Zero, FP.Zero, FP.Zero);
        
        /// <summary>
        /// 默认物理几何体
        /// </summary>
        private static BoxShape defaultShape = new(TSVector.one);
        
        /// <summary>
        /// 物理单位
        /// </summary>
        public RigidBody rigidbody { get; private set; }

        private Spatial spatial { get; set; }

        private TSVector mrigidbodyoffset = TSVector.zero;
        /// <summary>
        /// 物理单位平移偏移
        /// </summary>
        public TSVector rigidbodyoffset
        {
            get
            {
                return mrigidbodyoffset;
            }
            set
            {
                mrigidbodyoffset = value;
                rigidbody.Position = rigidbodyoffset + spatial.position;
            }
        }

        /// <summary>
        /// 是否为动态
        /// </summary>
        public bool kinematic
        {
            get
            {
                return rigidbody.IsKinematic;
            }
            set
            {
                rigidbody.IsKinematic = value;
            }
        }
        
        /// <summary>
        /// 是否为触发器
        /// </summary>
        public bool trigger
        {
            get
            {
                return rigidbody.IsColliderOnly;
            }
            set
            {
                rigidbody.IsColliderOnly = value;
            }
        }
        
        /// <summary>
        /// 立方体
        /// </summary>
        public BoxShape boxshape
        {
            get
            {
                var shape = rigidbody.Shape as BoxShape;
                if (null == shape)
                {
                    shape = new BoxShape(TSVector.one);
                    rigidbody.Shape = shape;
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
                var shape = rigidbody.Shape as SphereShape;
                if (null == shape)
                {
                    shape = new SphereShape(FP.One);
                    rigidbody.Shape = shape;
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
                var shape = rigidbody.Shape as CylinderShape;
                if (null == shape)
                {
                    shape = new CylinderShape(FP.One, FP.One);
                    rigidbody.Shape = shape;
                }
                
                return shape;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.Listen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            actor.stage.eventor.Listen<PhysCollisionEnterEvent>(OnCollisionEnter);
            actor.stage.eventor.Listen<PhysCollisionExitEvent>(OnCollisionExit);
            actor.stage.eventor.Listen<PhysTriggerEnterEvent>(OnTriggerEnter);
            actor.stage.eventor.Listen<PhysTriggerExitEvent>(OnTriggerExit);
            spatial = actor.GetBehavior<Spatial>();
            rigidbody = new RigidBody(defaultShape, defaultMaterial);
            actor.stage.phys.AddBody(actor.id, rigidbody);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.UnListen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            actor.stage.eventor.UnListen<PhysCollisionEnterEvent>(OnCollisionEnter);
            actor.stage.eventor.UnListen<PhysCollisionExitEvent>(OnCollisionExit);
            actor.stage.eventor.UnListen<PhysTriggerEnterEvent>(OnTriggerEnter);
            actor.stage.eventor.UnListen<PhysTriggerExitEvent>(OnTriggerExit);
            actor.stage.phys.RmvBody(actor.id);
        }

        private void OnSpatialPositionChanged(SpatialPositionChangedEvent e)
        {
            rigidbody.Position = rigidbodyoffset + e.position;
        }

        private void OnSpatialRotationChanged(SpatialRotationChangedEvent e)
        {
            rigidbody.Orientation = TSMatrix.CreateFromQuaternion(e.rotation);
        }

        private void OnCollisionEnter(PhysCollisionEnterEvent e)
        {
            if (actor.id != e.id0 && actor.id != e.id1) return;
            
            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionEnterEvent { actorId = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnCollisionExit(PhysCollisionExitEvent e)
        {
            if (actor.id != e.id0 && actor.id != e.id1) return;

            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionExitEvent { actorId = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnTriggerEnter(PhysTriggerEnterEvent e)
        {
            if (actor.id != e.id0 && actor.id != e.id1) return;

            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionEnterEvent { actorId = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnTriggerExit(PhysTriggerExitEvent e)
        {
            if (actor.id != e.id0 && actor.id != e.id1) return;

            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionExitEvent { actorId = selfIsId0 ? e.id1 : e.id0 });
        }
    }
}
