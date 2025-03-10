using Kowtow.Collision;
using Kowtow.Collision.Shapes;
using Kowtow.Math;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kowtow
{
    /// <summary>
    /// 世界
    /// </summary>
    public class World
    {
        /// <summary>
        /// 八叉树
        /// </summary>
        public Octree tree { get; private set; }
        /// <summary>
        /// 物理
        /// </summary>
        public Phys phys { get; private set; }
        /// <summary>
        /// 重力
        /// </summary>
        public FPVector3 gravity { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public FP timestep { get; private set; }
        /// <summary>
        /// 刚体列表
        /// </summary>
        private List<Rigidbody> rigidbodies = new();
        /// <summary>
        /// 世界构造函数
        /// </summary>
        /// <param name="gravity">重力</param>
        public World(FPVector3 gravity = default)
        {
            tree = new(this);
            phys = new(this, tree);

            this.gravity = gravity;
        }

        /// <summary>
        /// 添加刚体
        /// </summary>
        /// <param name="shape">几何体</param>
        /// <param name="mass">质量</param>
        /// <param name="material">物理材质</param>
        /// <returns>刚体</returns>
        public Rigidbody AddRigidbody(IShape shape, FP mass, Material material)
        {
            Rigidbody rigidbody = new(shape, mass, material);
            rigidbody.world = this;
            rigidbodies.Add(rigidbody);
            tree.Rigidbody2Node(rigidbody);

            return rigidbody;
        }

        /// <summary>
        /// 移除刚体
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        public void RmvRigidbody(Rigidbody rigidbody)
        {
            if (false == rigidbodies.Contains(rigidbody)) return;
            tree.RmvRigidbody(rigidbody);
            rigidbody.world = null;
            rigidbodies.Remove(rigidbody);
        }

        /// <summary>
        /// 驱动世界
        /// </summary>
        /// <param name="t">时间间隔</param>
        public void Update(FP t)
        {
            if (FP.Zero == t) return;
            timestep = t;

#if PARALLEL
            Parallel.ForEach(rigidbodies, rigidbody =>
            {
                rigidbody.Update(t);
            });
#else
            foreach (var rigidbody in rigidbodies)
            {
                rigidbody.Update(t);
            }
#endif
            Collisions();

            foreach (var rigidbody in rigidbodies)
            {
                // 事件通知
                rigidbody.NotifyColliderEvents();

                // AABB 更新树
                if (rigidbody.aabbupdated)
                {
                    tree.AABBUpdate(rigidbody);
                    rigidbody.aabbupdated = false;
                }
            }
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void Collisions()
        {
#if PARALLEL
            Parallel.ForEach(rigidbodies, self =>
            {
                self.ResetColliders();

                if (false == tree.QueryRigidbodies(self, out var bodies)) return;

                Parallel.ForEach(bodies, target =>
                {
                    Collision(self, target);
                });
            });
#else
            foreach (var self in rigidbodies)
            {
                self.ResetColliders();

                if (false == tree.QueryRigidbodies(self, out var bodies)) continue;

                foreach (var target in bodies)
                {
                    Collision(self, target);
                }
            }
#endif
        }

        private void Collision(Rigidbody self, Rigidbody target)
        {
            if (self == target) return;

            // 层级检测
            if (false == Layer.Query(self.layer, target.layer)) return;

            // TOI 检测/连续碰撞检测
            if (RigidbodyType.Dynamic == self.type && DetectionType.Continuous == self.detection)
            {
                ContinuousDetection(self, target);
                return;
            }

            // 离散碰撞检测
            DiscreteDetection(self, target);
        }

        /// <summary>
        /// 离散碰撞检测
        /// </summary>
        /// <param name="self">自身 Rigidbody</param>
        /// <param name="target">目标 Rigidbody</param>
        private void DiscreteDetection(Rigidbody self, Rigidbody target)
        {
            if (false == Detection.Detect(self, target, out var point, out var normal, out var penetration)) return;
            self.AddCollider(new Collider
            {
                rigidbody = target,
                point = point,
                normal = normal,
                penetration = penetration
            });
        }

        /// <summary>
        /// 连续碰撞检测
        /// </summary>
        /// <param name="self">自身 Rigidbody</param>
        /// <param name="target">目标 Rigidbody</param>
        private void ContinuousDetection(Rigidbody self, Rigidbody target)
        {
            // 获取刚体的运动起始和结束位置
            FPVector3 startA = self.position;
            FPVector3 endA = self.position + self.velocity * timestep;
            FPQuaternion startRotA = self.rotation;
            FPQuaternion endRotA = self.rotation;

            FPVector3 startB = target.position;
            FPVector3 endB = target.position + target.velocity * timestep;
            FPQuaternion startRotB = target.rotation;
            FPQuaternion endRotB = target.rotation;

            // 执行时间插值检测
            if (false == Detection.Sweep(self, target, startA, endA, startB, endB, startRotA, endRotA, startRotB, endRotB, out FP toi)) return;

            // 在碰撞时刻更新刚体状态
            FPVector3 collisionPositionA = FPVector3.Lerp(startA, endA, toi);
            FPQuaternion collisionRotationA = FPQuaternion.Slerp(startRotA, endRotA, toi);
            FPVector3 collisionPositionB = FPVector3.Lerp(startB, endB, toi);
            FPQuaternion collisionRotationB = FPQuaternion.Slerp(startRotB, endRotB, toi);

            // 计算碰撞点、法线、以及穿透深度
            if (false == Detection.Detect(self.shape, target.shape, collisionPositionA, collisionPositionB, collisionRotationA, collisionRotationB, out var point, out var normal, out var penetration)) return;

            FPVector3 correction = normal * penetration;
            self.position = collisionPositionA + correction;
            self.velocity -= self.velocity * timestep;

            // 添加碰撞器信息
            self.AddCollider(new Collider
            {
                rigidbody = target,
                point = point,
                normal = normal,
                penetration = penetration
            });
        }
    }
}
