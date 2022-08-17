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
    //public struct CollisionInfo
    //{
    //    public long self;
    //    public List<long> colliders;
    //}

    public abstract class Collider : PComp
    {
        public long Id { get; private set; }
        public Entity entity { get; protected set; }
        public List<long> colliderIds { get; private set; }

        //public event Action<CollisionInfo> collisionEnter;
        //public event Action<CollisionInfo> collisionUpdate;
        //public event Action<CollisionInfo> collisionLeave;

        public abstract Entity GenEntity();

        public Vector3 colliderPos { get; protected set; }
        public Vector3 colliderSize { get; protected set; }
        public abstract void ComputeCPS();

        protected override void OnCreate()
        {
            base.OnCreate();
            entity = GenEntity();
            Id = entity.InstanceId;
            RegisterDetectBody();
            Actor.ActorBehavior.Info.posChanged += PosChanged;
            Actor.ActorBehavior.Info.sizeChanged += SizeChanged;
            Engine.World.AddCollider(this);
        }

        protected override void OnDestroy()
        {
            Actor.ActorBehavior.Info.posChanged -= PosChanged;
            Actor.ActorBehavior.Info.sizeChanged -= SizeChanged;
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

        private void SizeChanged(Vector3 size)
        {
            ComputeCPS();
        }

        private void InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            if (null == colliderIds) colliderIds = new List<long>();
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);
            //if (null == collisionEnter) return;
            //collisionEnter(new CollisionInfo { self = Id, colliders = colliderIds });
        }

        private void CollisionEnded(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            colliderIds.Clear();
            //if (null == collisionLeave) return;
            //collisionLeave(new CollisionInfo { self = Id, colliders = colliderIds });
        }

        private void ContactCreated(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);

            //if (null == collisionUpdate) return;

            //collisionUpdate(
            //    new CollisionInfo
            //    {
            //        self = Id,
            //        colliders = colliderIds
            //    }
            //);
        }

        private void ContactRemoved(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            colliderIds = Engine.World.ConvEntityIds(sender.OverlappedEntities);

            //if (null == collisionUpdate) return;

            //collisionUpdate(
            //    new CollisionInfo
            //    {
            //        self = Id,
            //        colliders = Engine.World.ConvEntityIds(sender.OverlappedEntities)
            //    }
            //);
        }
    }
}
