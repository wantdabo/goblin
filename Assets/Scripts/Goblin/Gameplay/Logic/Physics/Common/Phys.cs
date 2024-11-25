using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow;
using Kowtow.Collision;
using Kowtow.Collision.Shapes;
using Kowtow.Math;
using System.Collections.Generic;
using System.Drawing;

namespace Goblin.Gameplay.Logic.Physics.Common
{
    /// <summary>
    /// 一帧碰撞几何体信息事件
    /// </summary>
    public struct PhysShapesEvent : IEvent
    {
        /// <summary>
        /// 碰撞几何体
        /// </summary>
        public (ushort type, IShape shape, FPVector3 position, FPQuaternion rotation)[] physinfos { get; set; }
    }

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
        /// 默认物理材质
        /// </summary>
        public Material defmaterial { get; private set; }
        /// <summary>
        /// 刚体列表
        /// </summary>
        private List<Rigidbody> rigidbodies = new();
        /// <summary>
        /// ActorId 对应的 RigidBody
        /// </summary>
        private Dictionary<uint, Rigidbody> abdict = new();
        /// <summary>
        /// Rigidbody 对应的 ActorId
        /// </summary>
        private Dictionary<Rigidbody, uint> badict = new();
        /// <summary>
        /// 碰撞几何体数据缓存
        /// </summary>
        private List<(ushort type, IShape shape, FPVector3 position, FPQuaternion rotation)> physinfos = new();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="gravity">重力</param>
        public void Initialize(FPVector3 gravity = default)
        {
            world = new World(gravity);
            defmaterial = new Material { friction = FP.Zero, bounciness = FP.Zero };
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            stage.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            stage.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
        }

        #region Body/物理单位操作
        /// <summary>
        /// 获取物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <returns>物理单位</returns>
        public Rigidbody GetBody(uint actorId)
        {
            if (false == abdict.TryGetValue(actorId, out var body)) return default;

            return body;
        }

        /// <summary>
        /// 获取 ActorID
        /// </summary>
        /// <param name="body">物理单位</param>
        /// <returns>ActorID</returns>
        public uint GetActorId(Rigidbody body)
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
        public void RmvBody(Rigidbody body)
        {
            if (null == body) return;

            var actorId = GetActorId(body);
            if (0 == actorId) return;

            world.RmvRigidbody(body);
            rigidbodies.Remove(body);
            abdict.Remove(actorId);
            badict.Remove(body);
        }

        /// <summary>
        /// 添加物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="shape">几何体</param>
        /// <param name="mass">质量</param>
        public Rigidbody AddBody(uint actorId, IShape shape, FP mass)
        {
            var body = AddBody(shape, mass);
            abdict.Add(actorId, body);
            badict.Add(body, actorId);

            return body;
        }

        /// <summary>
        /// 添加物理单位
        /// </summary>
        /// <param name="shape">几何体</param>
        /// <param name="mass">质量</param>
        public Rigidbody AddBody(IShape shape, FP mass)
        {
            var body = world.AddRigidbody(shape, mass, defmaterial);
            rigidbodies.Add(body);

            return body;
        }
        #endregion

        private (bool hit, (uint actorId, Collider collider)[] targets) HitResultConv(HitResult result)
        {
            if (false == result.hit) return (false, default);

            (uint actorId, Collider collider)[] colliders = new (uint actorId, Collider collider)[result.colliders.Count];
            for (int i = 0; i < result.colliders.Count; i++)
            {
                var rigidbody = result.colliders[i].rigidbody;
                var actorId = GetActorId(rigidbody);
                colliders[i] = (actorId, result.colliders[i]);
            }

            return (true, colliders);
        }

        /// <summary>
        /// 线段检测
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public (bool hit, (uint actorId, Collider collider)[] targets) Linecast(FPVector3 start, FPVector3 end, bool trigger = true, int layer = -1)
        {
            return HitResultConv(world.phys.Linecast(start, end, trigger, layer));
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="origin">原点</param>
        /// <param name="direction">方向</param>
        /// <param name="distance">距离</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public (bool hit, (uint actorId, Collider collider)[] targets) Raycast(FPVector3 origin, FPVector3 direction, FP distance, bool trigger = true, int layer = -1)
        {
            return HitResultConv(world.phys.Raycast(origin, direction, distance, trigger, layer));
        }

        /// <summary>
        /// 立方体检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="size">尺寸</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public (bool hit, (uint actorId, Collider collider)[] targets) OverlapBox(FPVector3 position, FPQuaternion rotation, FPVector3 size, bool trigger = true, int layer = -1)
        {
            return HitResultConv(world.phys.OverlapBox(position, rotation, size, trigger, layer));
        }

        /// <summary>
        /// 球体检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public (bool hit, (uint actorId, Collider collider)[] targets) OverlapSphere(FPVector3 position, FP radius, bool trigger = true, int layer = -1)
        {
            return HitResultConv(world.phys.OverlapSphere(position, radius, trigger, layer));
        }

        /// <summary>
        /// 圆柱体检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="height">高度</param>
        /// <param name="radius">半径</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public (bool hit, (uint actorId, Collider collider)[] targets) OverlapCylinder(FPVector3 position, FPQuaternion rotation, FP radius, FP height, bool trigger = true, int layer = -1)
        {
            return HitResultConv(world.phys.OverlapCylinder(position, rotation, radius, height, trigger, layer));
        }

        private void OnFPTick(FPTickEvent e)
        {
            world.Update(e.tick);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            foreach (var rigidbody in rigidbodies)
            {
                switch (rigidbody.layer)
                {
                    case Layer.Default:
                        break;
                    case Layer.Ground:
                        physinfos.Add((PHYS_SHAPE_DEFINE.GROUND, rigidbody.shape, rigidbody.position, rigidbody.rotation));
                        break;
                    case Layer.Player:
                        physinfos.Add((PHYS_SHAPE_DEFINE.PLAYER, rigidbody.shape, rigidbody.position, rigidbody.rotation));
                        break;
                }
            }

            stage.eventor.Tell(new PhysShapesEvent { physinfos = physinfos.ToArray() });
            physinfos.Clear();
        }
    }
}
