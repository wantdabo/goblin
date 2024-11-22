using Kowtow.Collision;
using Kowtow.Collision.Shapes;
using Kowtow.Math;
using System;
using System.Collections.Generic;

namespace Kowtow
{
    /// <summary>
    /// 刚体类型
    /// </summary>
    public enum RigidbodyType
    {
        /// <summary>
        /// 动态
        /// </summary>
        Dynamic,
        /// <summary>
        /// 静态
        /// </summary>
        Static,
    }

    /// <summary>
    /// 碰撞检测类型
    /// </summary>
    public enum DetectionType
    {
        /// <summary>
        /// 离散的
        /// </summary>
        Discrete,
        /// <summary>
        /// 连续的
        /// </summary>
        Continuous,
    }

    /// <summary>
    /// 刚体
    /// </summary>
    public class Rigidbody
    {
        /// <summary>
        /// 物理层
        /// </summary>
        public int layer { get; set; } = Layer.Default;
        /// <summary>
        /// 刚体类型
        /// </summary>
        public RigidbodyType type { get; set; } = RigidbodyType.Static;
        /// <summary>
        /// 碰撞检测类型
        /// </summary>
        public DetectionType detection { get; set; } = DetectionType.Discrete;
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; set; }
        /// <summary>
        /// 触发器 [开启后不会发生碰撞，但会触发事件]
        /// </summary>
        public bool trigger { get; set; }
        private IShape mshape { get; set; }
        /// <summary>
        /// 几何体
        /// </summary>
        public IShape shape
        {
            get
            {
                return mshape;
            }
            set
            {
                mshape = value;
                UpdateAABB();
            }
        }
        /// <summary>
        /// 包围盒
        /// </summary>
        public AABB aabb { get; set; }
        /// <summary>
        /// 质量
        /// </summary>
        public FP mass { get; set; } = FP.One;
        /// <summary>
        /// One Div Mass (1 / mass)
        /// </summary>
        public FP inverseMass { get { return FP.One / mass; } }
        /// <summary>
        /// 物理材质
        /// </summary>
        public Material material { get; set; }
        /// <summary>
        /// 重力缩放
        /// </summary>
        public FP gravityScale { get; set; } = FP.One;
        private FPVector3 mposition { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public FPVector3 position
        {
            get
            {
                return mposition;
            }
            set
            {
                mposition = value;
                UpdateAABB();
            }
        }
        private FPQuaternion mrotation { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public FPQuaternion rotation
        {
            get { return mrotation; }
            set
            {
                mrotation = value;
                UpdateAABB();
            }
        }
        /// <summary>
        /// 力
        /// </summary>
        public FPVector3 force { get; private set; }
        /// <summary>
        /// 速度阻力
        /// </summary>
        public FP drag { get; set; } = FP.Zero;
        /// <summary>
        /// 速度
        /// </summary>
        public FPVector3 velocity { get; set; }
        /// <summary>
        /// 上一帧碰撞关系列表
        /// </summary>
        private List<Collider> lastcolliders = new();
        /// <summary>
        /// 碰撞关系列表
        /// </summary>
        private List<Collider> colliders = new();
        /// <summary>
        /// AABB 变化了
        /// </summary>
        public bool aabbupdated { get; set; } = false;
        /// <summary>
        /// Collision 进入事件
        /// </summary>
        public event Action<Collider> CollisionEnter;
        /// <summary>
        /// Collision 持续事件
        /// </summary>
        public event Action<Collider> CollisionStay;
        /// <summary>
        /// Collision 离开事件
        /// </summary>
        public event Action<Collider> CollisionExit;
        /// <summary>
        /// Trigger 进入事件
        /// </summary>
        public event Action<Collider> TriggerEnter;
        /// <summary>
        /// Trigger 持续事件
        /// </summary>
        public event Action<Collider> TriggerStay;
        /// <summary>
        /// Trigger 离开事件
        /// </summary>
        public event Action<Collider> TriggerExit;

        /// <summary>
        /// 刚体构造函数
        /// </summary>
        /// <param name="shape">几何体</param>
        /// <param name="mass">质量</param>
        /// <param name="material">物理材质</param>
        public Rigidbody(IShape shape, FP mass, Material material)
        {
            this.shape = shape;
            this.mass = mass;
            this.material = material;
            UpdateAABB();
        }

        /// <summary>
        /// 更新 AABB 包围盒
        /// </summary>
        private void UpdateAABB()
        {
            aabb = AABB.CreateFromRigidbody(this);
            aabbupdated = true;
        }

        /// <summary>
        /// 施加力
        /// </summary>
        /// <param name="f">力</param>
        public void ApplyForce(FPVector3 f)
        {
            if (RigidbodyType.Static == type) return;

            force += f;
        }

        /// <summary>
        /// 施加冲量
        /// </summary>
        /// <param name="impulse">冲量</param>
        public void ApplyImpulse(FPVector3 impulse)
        {
            if (RigidbodyType.Static == type) return;

            velocity += impulse * inverseMass;
        }

        /// <summary>
        /// 获取碰撞列表
        /// </summary>
        /// <returns>碰撞列表</returns>
        public List<Collider> GetColliders()
        {
            return colliders;
        }

        /// <summary>
        /// 添加碰撞
        /// </summary>
        /// <param name="collider">碰撞关系</param>
        public void AddCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        /// <summary>
        /// 重置碰撞信息
        /// </summary>
        public void ResetColliders()
        {
            lastcolliders.Clear();
            lastcolliders.AddRange(colliders);
            colliders.Clear();
        }

        /// <summary>
        /// 通知碰撞事件
        /// </summary>
        public void NotifyColliderEvents()
        {
            foreach (var collider in colliders)
            {
                var lastcollider = lastcolliders.Find(last => collider.rigidbody == last.rigidbody);
                if (null != lastcollider.rigidbody)
                {
                    if (trigger)
                    {
                        TriggerStay?.Invoke(collider);
                    }
                    else
                    {
                        CollisionStay?.Invoke(collider);
                    }
                }
                else
                {
                    if (trigger)
                    {
                        TriggerEnter?.Invoke(collider);
                    }
                    else
                    {
                        CollisionEnter?.Invoke(collider);
                    }
                }
            }

            foreach (var lastcollider in lastcolliders)
            {
                var collider = colliders.Find(c => lastcollider.rigidbody == c.rigidbody);
                if (null == collider.rigidbody)
                {
                    if (trigger)
                    {
                        TriggerExit?.Invoke(lastcollider);
                    }
                    else
                    {
                        CollisionExit?.Invoke(lastcollider);
                    }
                }
            }
        }

        /// <summary>
        /// 计算重力
        /// </summary>
        private void GravityForce()
        {
            var gt = world.gravity * gravityScale;
            ApplyForce(gt);
        }

        /// <summary>
        /// 计算碰撞接触产生的作用力
        /// </summary>
        private void CollisionForce()
        {
            if (trigger || 0 == colliders.Count) return;

            foreach (var collider in colliders)
            {
                if (collider.rigidbody.trigger) continue;

                // 计算碰撞相对速度
                FPVector3 relativeVelocity = velocity - collider.rigidbody.velocity;

                // 碰撞速度沿法线分量
                FP velocityAlongNormal = FPVector3.Dot(relativeVelocity, collider.normal);

                // 如果分离状态，不做碰撞响应
                if (velocityAlongNormal > 0) continue;

                // 计算反弹速度（考虑材质弹力系数）
                FP bounciness = FPMath.Min(material.bounciness, collider.rigidbody.material.bounciness);
                FP impulseMagnitude = -(1 + bounciness) * velocityAlongNormal;
                impulseMagnitude /= inverseMass + collider.rigidbody.inverseMass;

                // 应用冲量
                FPVector3 impulse = impulseMagnitude * collider.normal;
                ApplyImpulse(impulse);

                // 计算摩擦力
                FPVector3 tangent = relativeVelocity - collider.normal * velocityAlongNormal;
                FP frictionMagnitude = tangent.magnitude * material.friction * collider.rigidbody.material.friction;

                if (frictionMagnitude > 0)
                {
                    // 摩擦力方向与切向速度方向相反
                    FPVector3 frictionForce = -tangent.normalized * frictionMagnitude;
                    ApplyForce(frictionForce);
                }

                // 如果是连续碰撞检测, 不需要额外修正, 直接跳过 (在连续碰撞检测过程中已经修正)
                if (DetectionType.Continuous == detection) continue;

                // 穿透修正
                FPVector3 correction = collider.normal * collider.penetration;
                position += correction;
            }
        }

        /// <summary>
        /// 计算速度
        /// </summary>
        private void Force2Velocity(FP t)
        {
            // 计算加速
            var acceleration = force * inverseMass;
            force = FPVector3.zero;
            // 增加速度
            velocity += acceleration * t;
            // 速度阻尼
            velocity *= (FP.One - drag * t);
        }

        /// <summary>
        /// 驱动刚体
        /// </summary>
        /// <param name="t">时间间隔</param>
        public void Update(FP t)
        {
            if (RigidbodyType.Static == type) return;

            // 计算重力
            GravityForce();
            // 计算速度
            Force2Velocity(t);
            // 计算碰撞接触产生的作用力
            CollisionForce();

            // 更新位置
            position += velocity * t;
        }
    }
}
