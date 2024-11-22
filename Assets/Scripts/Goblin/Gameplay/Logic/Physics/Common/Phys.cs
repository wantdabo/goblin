using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow;
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
        /// ActorId 对应的 RigidBody
        /// </summary>
        private Dictionary<uint, Rigidbody> abdict = new();
        /// <summary>
        /// Rigidbody 对应的 ActorId
        /// </summary>
        private Dictionary<Rigidbody, uint> badict = new();
        /// <summary>
        /// ActorID 临时列表
        /// </summary>
        private List<uint> temps = new();
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
            abdict.Remove(actorId);
            badict.Remove(body);
        }

        /// <summary>
        /// 添加物理单位
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="body">物理单位</param>
        public Rigidbody AddBody(uint actorId, IShape shape, FP mass, Material material)
        {
            var body = world.AddRigidbody(shape, mass, material);
            abdict.Add(actorId, body);
            badict.Add(body, actorId);

            return body;
        }
        #endregion

        private void OnFPTick(FPTickEvent e)
        {
            world.Update(e.tick);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            foreach (var rigidbody in abdict.Values) physinfos.Add((PHYS_SHAPE_DEFINE.PLAYER, rigidbody.shape, rigidbody.position, rigidbody.rotation));
            stage.eventor.Tell(new PhysShapesEvent { physinfos = physinfos.ToArray() });
            physinfos.Clear();
        }
    }
}
