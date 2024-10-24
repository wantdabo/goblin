using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;
using System.Drawing;
using TrueSync;
using TrueSync.Physics3D;

namespace Goblin.Gameplay.Logic.Physics.Common
{
    public struct PhysCollisionEnterEvent : IEvent
    {
        public uint id0 { get; set; }
        public uint id1 { get; set; }
    }

    public struct PhysCollisionExitEvent : IEvent
    {
        public uint id0 { get; set; }
        public uint id1 { get; set; }
    }

    public struct PhysTriggerEnterEvent : IEvent
    {
        public uint id0 { get; set; }
        public uint id1 { get; set; }
    }

    public struct PhysTriggerExitEvent : IEvent
    {
        public uint id0 { get; set; }
        public uint id1 { get; set; }
    }

    public class Phys : Comp
    {
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }
        /// <summary>
        /// 物理世界
        /// </summary>
        private World world { get; set; }
        /// <summary>
        /// ActorId 对应的 RigidBody
        /// </summary>
        private Dictionary<uint, RigidBody> abdict = new();
        /// <summary>
        /// Rigidbody 对应的 ActorId
        /// </summary>
        private Dictionary<RigidBody, uint> badict = new();
        /// <summary>
        /// ActorID 临时列表
        /// </summary>
        private List<uint> actorIdTemps = new();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="gravity">重力</param>
        /// <typeparam name="T">碰撞检测类型</typeparam>
        public void Initialize<T>(TSVector gravity = default) where T : CollisionSystem, new()
        {
            world = new World(new T());
            world.CollisionSystem.world = world;
            world.Gravity = gravity;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            world.Events.BodiesBeginCollide += CollisionEnter;
            world.Events.BodiesEndCollide += CollisionExit;
            world.Events.TriggerBeginCollide += TriggerEnter;
            world.Events.TriggerEndCollide += TriggerExit;
            world.Events.RemovedRigidBody += OnRemovedRigidBody;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            world.Events.BodiesBeginCollide -= CollisionEnter;
            world.Events.BodiesEndCollide -= CollisionExit;
            world.Events.TriggerBeginCollide -= TriggerEnter;
            world.Events.TriggerEndCollide -= TriggerExit;
            world.Events.RemovedRigidBody -= OnRemovedRigidBody;
            world.Clear();
        }

        #region Body/物理单位操作
        public RigidBody GetBody(uint actorId)
        {
            if (false == abdict.TryGetValue(actorId, out var body)) return default;

            return body;
        }

        public uint GetActorId(RigidBody body)
        {
            if (false == badict.TryGetValue(body, out var actorId)) return default;

            return actorId;
        }

        public void RmvBody(uint actorId)
        {
            if (0 == actorId) return;

            RmvBody(GetBody(actorId));
        }

        public void RmvBody(RigidBody body)
        {
            if (null == body) return;

            var actorId = GetActorId(body);
            if (0 == actorId) return;

            world.RemoveBody(body);
            abdict.Remove(actorId);
            badict.Remove(body);
        }

        public void AddBody(uint actorId, RigidBody body)
        {
            abdict.Add(actorId, body);
            badict.Add(body, actorId);
            world.AddBody(body);
        }
        #endregion

        #region 碰撞检测算法
        public (bool hit, uint actorId) OverlapBox(uint cullActor, TSVector point, TSVector size, TSQuaternion orientation)
        {
            BoxShape shape = new(size);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(orientation), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }
        
        public (bool hit, uint[] actorIds) OverlapBoxs(uint cullActor, TSVector point, TSVector size, TSQuaternion orientation)
        {
            BoxShape shape = new(size);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(orientation), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    actorIdTemps.Add(kv.Key);
                }
            }

            return (actorIdTemps.Count > 0, actorIdTemps.ToArray());
        }
        
        public (bool hit, uint actorId) OverlapSphere(uint cullActor, TSVector point, FP radius)
        {
            SphereShape shape = new(radius);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(TSQuaternion.identity), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }
        
        public (bool hit, uint[] actorIds) OverlapSpheres(uint cullActor, TSVector point, FP radius)
        {
            SphereShape shape = new(radius);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(TSQuaternion.identity), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    actorIdTemps.Add(kv.Key);
                }
            }

            return (actorIdTemps.Count > 0, actorIdTemps.ToArray());
        }
        
        public (bool hit, uint actorId) OverlapCylinder(uint cullActor, TSVector point, FP radius, FP height, TSQuaternion orientation)
        {
            CylinderShape shape = new(radius, height);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(orientation), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }
        
        public (bool hit, uint[] actorIds) OverlapCylinders(uint cullActor, TSVector point, FP radius, FP height, TSQuaternion orientation)
        {
            CylinderShape shape = new(radius, height);
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, kv.Value.Orientation, TSMatrix.CreateFromQuaternion(orientation), kv.Value.Position, point))
                {
                    if (cullActor == kv.Key) continue;

                    actorIdTemps.Add(kv.Key);
                }
            }

            return (actorIdTemps.Count > 0, actorIdTemps.ToArray());
        }

        private bool Overlap(ISupportMappable ashape, ISupportMappable bshape, TSMatrix aorie, TSMatrix borie, TSVector apos, TSVector bpos)
        {
            return XenoCollide.Detect(ashape, bshape, ref aorie, ref borie, ref apos, ref bpos, out var point, out var normal, out var penetration);
        }
        #endregion

        private void OnFPTick(FPTickEvent e)
        {
            world.Step(e.tick);
        }

        #region Rigibody 事件
        private void CollisionEnter(Contact c)
        {
            stage.eventor.Tell(new PhysCollisionEnterEvent { id0 = GetActorId(c.body1), id1 = GetActorId(c.body2) });
        }

        private void CollisionExit(RigidBody body1, RigidBody body2)
        {
            stage.eventor.Tell(new PhysCollisionExitEvent { id0 = GetActorId(body1), id1 = GetActorId(body2) });
        }

        private void TriggerEnter(Contact c)
        {
            stage.eventor.Tell(new PhysTriggerEnterEvent { id0 = GetActorId(c.body1), id1 = GetActorId(c.body2) });
        }

        private void TriggerExit(RigidBody body1, RigidBody body2)
        {
            stage.eventor.Tell(new PhysTriggerExitEvent { id0 = GetActorId(body1), id1 = GetActorId(body2) });
        }

        private void OnRemovedRigidBody(RigidBody body)
        {
        }
        #endregion
    }
}
