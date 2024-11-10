using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;
using System.Drawing;
using TrueSync;
using TrueSync.Physics3D;

namespace Goblin.Gameplay.Logic.Physics.Common
{
    #region 物理事件
    /// <summary>
    /// 一帧碰撞几何体信息事件
    /// </summary>
    public struct PhysShapesEvent : IEvent
    {
        /// <summary>
        /// 碰撞几何体
        /// </summary>
        public (ushort type, Shape shape, TSVector position, TSQuaternion rotation)[] physinfos { get; set; }
    }

    /// <summary>
    /// 物理碰撞进入事件
    /// </summary>
    public struct PhysCollisionEnterEvent : IEvent
    {
        /// <summary>
        /// ActorID A
        /// </summary>
        public uint id0 { get; set; }
        /// <summary>
        /// ActorID B
        /// </summary>
        public uint id1 { get; set; }
    }

    /// <summary>
    /// 物理碰撞退出事件
    /// </summary>
    public struct PhysCollisionExitEvent : IEvent
    {
        /// <summary>
        /// ActorID A
        /// </summary>
        public uint id0 { get; set; }
        /// <summary>
        /// ActorID B
        /// </summary>
        public uint id1 { get; set; }
    }

    /// <summary>
    /// 物理触发进入事件
    /// </summary>
    public struct PhysTriggerEnterEvent : IEvent
    {
        /// <summary>
        /// ActorID A
        /// </summary>
        public uint id0 { get; set; }
        /// <summary>
        /// ActorID B
        /// </summary>
        public uint id1 { get; set; }
    }

    /// <summary>
    /// 物理触发退出事件
    /// </summary>
    public struct PhysTriggerExitEvent : IEvent
    {
        /// <summary>
        /// ActorID A
        /// </summary>
        public uint id0 { get; set; }
        /// <summary>
        /// ActorID B
        /// </summary>
        public uint id1 { get; set; }
    }
    #endregion

    /// <summary>
    /// 物理
    /// </summary>
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
        private List<uint> temps = new();
        /// <summary>
        /// 碰撞几何体数据缓存
        /// </summary>
        private List<(ushort type, Shape shape, TSVector position, TSQuaternion rotation)> physinfos = new();

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
            world.Events.BodiesBeginCollide += CollisionEnter;
            world.Events.BodiesEndCollide += CollisionExit;
            world.Events.TriggerBeginCollide += TriggerEnter;
            world.Events.TriggerEndCollide += TriggerExit;
            world.Events.RemovedRigidBody += OnRemovedRigidBody;
            stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            stage.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.Events.BodiesBeginCollide -= CollisionEnter;
            world.Events.BodiesEndCollide -= CollisionExit;
            world.Events.TriggerBeginCollide -= TriggerEnter;
            world.Events.TriggerEndCollide -= TriggerExit;
            world.Events.RemovedRigidBody -= OnRemovedRigidBody;
            world.Clear();
            stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            stage.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
        }

        #region Body/物理单位操作
        /// <summary>
        /// 获取物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <returns>物理单位</returns>
        public RigidBody GetBody(uint actorId)
        {
            if (false == abdict.TryGetValue(actorId, out var body)) return default;

            return body;
        }

        /// <summary>
        /// 获取 ActorID
        /// </summary>
        /// <param name="body">物理单位</param>
        /// <returns>ActorID</returns>
        public uint GetActorId(RigidBody body)
        {
            if (false == badict.TryGetValue(body, out var actorId)) return default;

            return actorId;
        }

        /// <summary>
        /// 移除物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        public void RmvBody(uint actorId)
        {
            if (0 == actorId) return;

            RmvBody(GetBody(actorId));
        }

        /// <summary>
        /// 移除物理单位
        /// </summary>
        /// <param name="body">物理单位</param>
        public void RmvBody(RigidBody body)
        {
            if (null == body) return;

            var actorId = GetActorId(body);
            if (0 == actorId) return;

            world.RemoveBody(body);
            abdict.Remove(actorId);
            badict.Remove(body);
        }

        /// <summary>
        /// 添加物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="body">物理单位</param>
        public void AddBody(uint actorId, RigidBody body)
        {
            abdict.Add(actorId, body);
            badict.Add(body, actorId);
            world.AddBody(body);
        }
        #endregion

        #region 碰撞检测算法
        /// <summary>
        /// 立方体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="rotation">方向</param>
        /// <returns>(YES/NO, ActorID)</returns>
        public (bool hit, uint actorId) OverlapBox(TSVector point, TSVector size, TSQuaternion rotation)
        {
            BoxShape shape = new(size);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, rotation));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(rotation), kv.Value.Orientation, kv.Value.Position, point))
                {
                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }

        /// <summary>
        /// 群体立方体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="size">尺寸</param>
        /// <param name="rotation">方向</param>
        /// <returns>(YES/NO, ActorID[])</returns>
        public (bool hit, uint[] actorIds) OverlapBoxs(TSVector point, TSVector size, TSQuaternion rotation)
        {
            BoxShape shape = new(size);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, rotation));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(rotation), kv.Value.Orientation, kv.Value.Position, point))
                {
                    temps.Add(kv.Key);
                }
            }

            return (temps.Count > 0, temps.ToArray());
        }

        /// <summary>
        /// 球体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="radius">半径</param>
        /// <returns>(YES/NO, ActorID)</returns>
        public (bool hit, uint actorId) OverlapSphere(TSVector point, FP radius)
        {
            SphereShape shape = new(radius);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, TSQuaternion.identity));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(TSQuaternion.identity), kv.Value.Orientation, kv.Value.Position, point))
                {
                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }

        /// <summary>
        /// 群体球体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="radius">半径</param>
        /// <returns>(YES/NO, ActorID[])</returns>
        public (bool hit, uint[] actorIds) OverlapSpheres(TSVector point, FP radius)
        {
            SphereShape shape = new(radius);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, TSQuaternion.identity));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(TSQuaternion.identity), kv.Value.Orientation, kv.Value.Position, point))
                {
                    temps.Add(kv.Key);
                }
            }

            return (temps.Count > 0, temps.ToArray());
        }

        /// <summary>
        /// 圆柱体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高度</param>
        /// <param name="rotation">方向</param>
        /// <returns>(YES/NO, ActorID)</returns>
        public (bool hit, uint actorId) OverlapCylinder(TSVector point, FP radius, FP height, TSQuaternion rotation)
        {
            CylinderShape shape = new(radius, height);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, rotation));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(rotation), kv.Value.Orientation, kv.Value.Position, point))
                {
                    return (true, kv.Key);
                }
            }

            return (false, 0);
        }

        /// <summary>
        /// 群体圆柱体碰撞检测
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高度</param>
        /// <param name="rotation">方向</param>
        /// <returns>(YES/NO, ActorID[])</returns>
        public (bool hit, uint[] actorIds) OverlapCylinders(TSVector point, FP radius, FP height, TSQuaternion rotation)
        {
            CylinderShape shape = new(radius, height);
            physinfos.Add((PHYS_SHAPE_DEFINE.OVERLAP, shape, point, rotation));
            
            temps.Clear();
            foreach (var kv in abdict)
            {
                if (Overlap(shape, kv.Value.Shape, TSMatrix.CreateFromQuaternion(rotation), kv.Value.Orientation, kv.Value.Position, point))
                {
                    temps.Add(kv.Key);
                }
            }

            return (temps.Count > 0, temps.ToArray());
        }

        /// <summary>
        /// 几何体碰撞检测
        /// </summary>
        /// <param name="ashape">几何体 A</param>
        /// <param name="bshape">几何体 B</param>
        /// <param name="aorie">几何体 A 方向</param>
        /// <param name="borie">几何体 B 方向</param>
        /// <param name="apos">几何体 A 位置</param>
        /// <param name="bpos">几何体 B 位置</param>
        /// <returns>YES/NO</returns>
        private bool Overlap(ISupportMappable ashape, ISupportMappable bshape, TSMatrix aorie, TSMatrix borie, TSVector apos, TSVector bpos)
        {
            return XenoCollide.Detect(ashape, bshape, ref aorie, ref borie, ref apos, ref bpos, out var point, out var normal, out var penetration);
        }
        #endregion

        private void OnFPTick(FPTickEvent e)
        {
            world.Step(e.tick);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            foreach (var rigidbody in abdict.Values) physinfos.Add((PHYS_SHAPE_DEFINE.PLAYER, rigidbody.Shape,rigidbody.Position, TSQuaternion.CreateFromMatrix(rigidbody.Orientation)));
            stage.eventor.Tell(new PhysShapesEvent { physinfos = physinfos.ToArray() });
            physinfos.Clear();
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
