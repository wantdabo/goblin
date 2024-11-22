using Kowtow.Collision.Shapes;
using Kowtow.Math;
using System.Collections.Generic;

namespace Kowtow.Collision
{
    /// <summary>
    /// 碰撞结果
    /// </summary>
    public struct HitResult
    {
        /// <summary>
        /// 是否碰撞
        /// </summary>
        public bool hit { get; set; }
        /// <summary>
        /// 碰撞列表
        /// </summary>
        public List<Collider> colliders { get; set; }
    }

    /// <summary>
    /// 物理方法
    /// </summary>
    public class Phys
    {
        /// <summary>
        /// 世界
        /// </summary>
        private World world { get; set; }
        /// <summary>
        /// 树
        /// </summary>
        private Octree tree { get; set; }

        /// <summary>
        /// 物理构造函数
        /// </summary>
        /// <param name="world">世界</param>
        /// <param name="tree">树</param>
        public Phys(World world, Octree tree)
        {
            this.world = world;
            this.tree = tree;
        }

        /// <summary>
        /// 线段检测
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult Linecast(FPVector3 start, FPVector3 end, bool trigger = true, int layer = -1)
        {
            FPVector3 min = FPVector3.Min(start, end);
            FPVector3 max = FPVector3.Max(start, end);

            AABB aabb = new AABB()
            {
                position = (min + max) * FP.Half,
                size = max - min
            };

            if (false == tree.QueryRigidbodies(aabb, out List<Rigidbody> rigidbodies))
            {
                return default;
            }

            HitResult result = new();
            foreach (var rigidbody in rigidbodies)
            {
                if (RigidbodyFilter(rigidbody, trigger, layer)) continue;

                if (Detection.DetectLineShape(start, end, rigidbody.shape, rigidbody.position, rigidbody.rotation, out var point, out var normal, out var penetration))
                {
                    result.hit = true;
                    if (null == result.colliders) result.colliders = new();
                    result.colliders.Add(new Collider()
                    {
                        rigidbody = rigidbody,
                        point = point,
                        normal = normal,
                        penetration = penetration
                    });
                }
            }

            return result;
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
        public HitResult Raycast(FPVector3 origin, FPVector3 direction, FP distance, bool trigger = true, int layer = -1)
        {
            return Linecast(origin, origin + direction * distance, trigger, layer);
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
        public HitResult OverlapBox(FPVector3 position, FPQuaternion rotation, FPVector3 size, bool trigger = true, int layer = -1)
        {
            BoxShape box = new BoxShape(position, size);
            var aabb = AABB.CreateFromShape(box, position, rotation);
            if (false == tree.QueryRigidbodies(aabb, out List<Rigidbody> rigidbodies))
            {
                return default;
            }

            HitResult result = new();
            foreach (var rigidbody in rigidbodies)
            {
                if (RigidbodyFilter(rigidbody, trigger, layer)) continue;

                if (Detection.Detect(box, rigidbody.shape, position, rigidbody.position, rotation, rigidbody.rotation, out var point, out var normal, out var penetration))
                {
                    result.hit = true;
                    if (null == result.colliders) result.colliders = new();
                    result.colliders.Add(new Collider()
                    {
                        rigidbody = rigidbody,
                        point = point,
                        normal = normal,
                        penetration = penetration
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 球体检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="trigger">检测 Trigger</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult OverlapSphere(FPVector3 position, FP radius, bool trigger = true, int layer = -1)
        {
            SphereShape sphere = new SphereShape(position, radius);
            var aabb = AABB.CreateFromShape(sphere, position, FPQuaternion.identity);
            if (false == tree.QueryRigidbodies(aabb, out List<Rigidbody> rigidbodies))
            {
                return default;
            }

            HitResult result = new();
            foreach (var rigidbody in rigidbodies)
            {
                if (RigidbodyFilter(rigidbody, trigger, layer)) continue;

                if (Detection.Detect(sphere, rigidbody.shape, position, rigidbody.position, FPQuaternion.identity, rigidbody.rotation, out var point, out var normal, out var penetration))
                {
                    result.hit = true;
                    if (null == result.colliders) result.colliders = new();
                    result.colliders.Add(new Collider()
                    {
                        rigidbody = rigidbody,
                        point = point,
                        normal = normal,
                        penetration = penetration
                    });
                }
            }

            return result;
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
        public HitResult OverlapCylinder(FPVector3 position, FPQuaternion rotation, FP height, FP radius, bool trigger = true, int layer = -1)
        {
            CylinderShape cylinder = new CylinderShape(position, height, radius);
            var aabb = AABB.CreateFromShape(cylinder, position, rotation);
            if (false == tree.QueryRigidbodies(aabb, out List<Rigidbody> rigidbodies))
            {
                return default;
            }

            HitResult result = new();
            foreach (var rigidbody in rigidbodies)
            {
                if (RigidbodyFilter(rigidbody, trigger, layer)) continue;

                if (Detection.Detect(cylinder, rigidbody.shape, position, rigidbody.position, rotation, rigidbody.rotation, out var point, out var normal, out var penetration))
                {
                    result.hit = true;
                    if (null == result.colliders) result.colliders = new();
                    result.colliders.Add(new Collider()
                    {
                        rigidbody = rigidbody,
                        point = point,
                        normal = normal,
                        penetration = penetration
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 检测过滤
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        /// <param name="trigger">Trigger 过滤</param>
        /// <param name="layer">层级过滤</param>
        /// <returns>YES/NO (YES 表示过滤成立, NO 表示过滤不成立; 成立表示不执行)</returns>
        private bool RigidbodyFilter(Rigidbody rigidbody, bool trigger, int layer)
        {
            if (false == trigger && rigidbody.trigger) return true;
            if (-1 != layer && layer != rigidbody.layer) return true;

            return false;
        }
    }
}
