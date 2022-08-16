using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics
{
    public class World : PComp, IPLateLoop
    {
        public Vector3 gravity = new Vector3(0, 981 * Fix64.EN2, 0);
        public Space Space;

        protected override void OnCreate()
        {
            base.OnCreate();
            Space = new Space();
            Space.ForceUpdater.Gravity = gravity;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public Collider GetCollider(long id) 
        {
            colliderDict.TryGetValue(id, out var collider);

            return collider;
        }

        private Dictionary<long, Collider> colliderDict = new Dictionary<long, Collider>();
        public void AddCollider(Collider collider) 
        {
            colliderDict.Add(collider.entity.InstanceId, collider);
            Space.Add(collider.entity);
        }

        public void RmvCollider(Collider collider) 
        {
            Space.Remove(collider.entity);
            colliderDict.Remove(collider.entity.InstanceId);
        }

        public List<long> ConvEntityIds(EntityCollidableCollection entitys)
        {
            List<long> ids = new List<long>();
            foreach (var entity in entitys) ids.Add(entity.InstanceId);

            return ids;
        }

        public List<Collider> ConvEntityColliders(EntityCollidableCollection entitys) 
        {
            List<Collider> colliders = new List<Collider>();
            foreach (var entity in entitys) colliders.Add(GetCollider(entity.InstanceId));

            return colliders;
        }

        public List<Collider> ConvEntityColliders(List<long> ids) 
        {
            List<Collider> colliders = new List<Collider>();
            foreach (var id in ids) colliders.Add(GetCollider(id));

            return colliders;
        }

        public void PLateLoop(int frame, Fix64 detailTime)
        {
            Space.Update(detailTime);
        }
    }
}
