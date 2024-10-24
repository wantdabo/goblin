using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;
using TrueSync.Physics3D;

namespace Goblin.Gameplay.Logic.Physics
{
    public struct CollisionEnterEvent : IEvent
    {
        public uint self { get; set; }
        public uint target { get; set; }
    }

    public struct CollisionExitEvent : IEvent
    {
        public uint self { get; set; }
        public uint target { get; set; }
    }

    public class PhysAgent : Behavior
    {
        private static BodyMaterial defaultMaterial = new(FP.Zero, FP.Zero, FP.Zero);
        private static BoxShape defaultShape = new(TSVector.one);

        public RigidBody rigidbody { get; private set; }
        
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

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<SpatialPositionChangedEvent>(OnSpatialPositionChanged);
            actor.eventor.Listen<SpatialRotationChangedEvent>(OnSpatialRotationChanged);
            actor.stage.eventor.Listen<PhysCollisionEnterEvent>(OnCollisionEnter);
            actor.stage.eventor.Listen<PhysCollisionExitEvent>(OnCollisionExit);
            actor.stage.eventor.Listen<PhysTriggerEnterEvent>(OnTriggerEnter);
            actor.stage.eventor.Listen<PhysTriggerExitEvent>(OnTriggerExit);
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
            rigidbody.Position = e.position;
        }
        
        private void OnSpatialRotationChanged(SpatialRotationChangedEvent e)
        {
            rigidbody.Orientation = TSMatrix.CreateFromQuaternion(e.rotation);
        }

        private void OnCollisionEnter(PhysCollisionEnterEvent e)
        {
            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionEnterEvent { self = selfIsId0 ? e.id0 : e.id1, target = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnCollisionExit(PhysCollisionExitEvent e)
        {
            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionExitEvent { self = selfIsId0 ? e.id0 : e.id1, target = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnTriggerEnter(PhysTriggerEnterEvent e)
        {
            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionEnterEvent { self = selfIsId0 ? e.id0 : e.id1, target = selfIsId0 ? e.id1 : e.id0 });
        }

        private void OnTriggerExit(PhysTriggerExitEvent e)
        {
            var selfIsId0 = e.id0 == actor.id;
            actor.eventor.Tell(new CollisionEnterEvent { self = selfIsId0 ? e.id0 : e.id1, target = selfIsId0 ? e.id1 : e.id0 });
        }
    }
}
