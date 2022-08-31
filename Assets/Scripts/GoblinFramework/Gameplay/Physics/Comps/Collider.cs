using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Comps
{
    public abstract class Collider : PComp
    {
        public long Id { get; private set; }
        public Entity entity { get; protected set; }
        public List<long> colliderIds { get; private set; }

        public abstract Entity GenEntity();

        public Vector3 colliderPos { get; protected set; }
        public Vector3 colliderRotation { get; protected set; }
        public Vector3 colliderScale { get; protected set; }
        public abstract void ComputeCPS();

        protected override void OnCreate()
        {
            base.OnCreate();
            entity = GenEntity();
            Id = entity.InstanceId;
            RegisterDetectBody();
            actor.actorBehaivor.info.posChanged += PosChanged;
            actor.actorBehaivor.info.rotationChanged += RotationChanged;
            actor.actorBehaivor.info.scaleChanged += ScaleChanged;
            Engine.World.AddCollider(this);
        }

        protected override void OnDestroy()
        {
            actor.actorBehaivor.info.posChanged -= PosChanged;
            actor.actorBehaivor.info.rotationChanged -= RotationChanged;
            actor.actorBehaivor.info.scaleChanged -= ScaleChanged;
            UnRegisterDetectBody();
            Engine.World.RmvCollider(this);
            Id = -1;
            base.OnDestroy();
        }

        private void RegisterDetectBody()
        {
            entity.CollisionInformation.Events.InitialCollisionDetected += InitialCollisionDetected;
            entity.CollisionInformation.Events.ContactCreated += ContactCreated;
            entity.CollisionInformation.Events.ContactRemoved += ContactRemoved;
            entity.CollisionInformation.Events.CollisionEnded += CollisionEnded;
        }

        private void UnRegisterDetectBody()
        {
            entity.CollisionInformation.Events.InitialCollisionDetected -= InitialCollisionDetected;
            entity.CollisionInformation.Events.ContactCreated -= ContactCreated;
            entity.CollisionInformation.Events.ContactRemoved -= ContactRemoved;
            entity.CollisionInformation.Events.CollisionEnded -= CollisionEnded;
        }

        private void PosChanged(Vector3 pos)
        {
            ComputeCPS();
        }

        private void RotationChanged(Vector3 rotation)
        {
            ComputeCPS();
        }

        private void ScaleChanged(Vector4 scale)
        {
            ComputeCPS();
        }

        private void InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            if (null == colliderIds) colliderIds = new List<long>();
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);
        }

        private void CollisionEnded(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);
        }

        private void ContactCreated(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);
        }

        private void ContactRemoved(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);
        }
    }
}
