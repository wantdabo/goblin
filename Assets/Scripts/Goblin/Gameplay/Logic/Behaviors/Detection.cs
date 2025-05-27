using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Collisions;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
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
        public List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)> colliders { get; set; }
    }
    
    /// <summary>
    /// 碰撞检测
    /// </summary>
    public class Detection : Behavior
    {
        /// <summary>
        /// 判断 AABB 是否包含另一个 AABB
        /// </summary>
        /// <param name="container">AABB (容纳区域)</param>
        /// <param name="item">AABB (容纳单位)</param>
        /// <returns>YES/NO</returns>
        public bool Inside(AABB container, AABB item)
        {
            FPVector3 min1 = container.position - container.size * FP.Half;
            FPVector3 max1 = container.position + container.size * FP.Half;
            FPVector3 min2 = item.position - item.size * FP.Half;
            FPVector3 max2 = item.position + item.size * FP.Half;

            return
            (
                min1.x <= min2.x && min1.y <= min2.y && min1.z <= min2.z
                &&
                max1.x >= max2.x && max1.y >= max2.y && max1.z >= max2.z
            ); 
        }
        
        /// <summary>
        /// 检测两个碰撞盒之间的重叠
        /// </summary>
        /// <param name="a">碰撞盒 A</param>
        /// <param name="b">碰撞盒 B</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从 BoxB 指向 BoxA 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Overlap(ColliderInfo a, ColliderInfo b, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 如果两个碰撞盒的层不匹配，则不进行碰撞检测
            if (false == COLLIDER_DEFINE.QUERY(a.layer, b.layer)) return false;

            // 获取空间信息
            if (false == stage.SeekBehaviorInfo(a.id, out SpatialInfo aspatial) || false == stage.SeekBehaviorInfo(b.id, out SpatialInfo bspatial)) return false;

            var apos = aspatial.position;
            var arot = FPQuaternion.Euler(aspatial.euler);
            var bpos = bspatial.position;
            var brot = FPQuaternion.Euler(bspatial.euler);

            // 根据碰撞盒的形状进行不同的碰撞检测
            switch (a.shape)
            {
                case COLLIDER_DEFINE.BOX:
                    switch (b.shape)
                    {
                        case COLLIDER_DEFINE.BOX:
                            return Detect(a.box, b.box, apos, bpos, arot, brot, out point, out normal, out penetration);
                        case COLLIDER_DEFINE.SPHERE:
                            return Detect(a.box, b.sphere, apos, bpos, arot, out point, out normal, out penetration);
                    }
                    break;
                case COLLIDER_DEFINE.SPHERE:
                    switch (b.shape)
                    {
                        case COLLIDER_DEFINE.BOX:
                            return Detect(b.box, a.sphere, bpos, apos, brot, out point, out normal, out penetration);
                        case COLLIDER_DEFINE.SPHERE:
                            return Detect(a.sphere, b.sphere, apos, bpos, out point, out normal, out penetration);
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// ColliderInfo 检测
        /// </summary>
        /// <param name="collider">碰撞盒</param>
        /// <returns>结果</returns>
        public HitResult Overlap(ColliderInfo collider)
        {
            if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) return default;

            HitResult result = new();
            switch (collider.shape)
            {
                case COLLIDER_DEFINE.BOX:
                    result = OverlapBox(spatial.position, FPQuaternion.Euler(spatial.euler), collider.box.size, collider.layer);
                    break;
                case COLLIDER_DEFINE.SPHERE:
                    result = OverlapSphere(spatial.position, collider.sphere.radius, collider.layer);
                    break;
            }

            // 如果发生碰撞，清除当前碰撞体
            if (result.hit)
            {
                var colliders = ObjectCache.Ensure<List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)>>();
                foreach (var c in result.colliders)
                {
                    if (collider.id == c.id) continue;
                    colliders.Add(c);
                }
                result.colliders.Clear();
                ObjectCache.Set(result.colliders);
                
                result.colliders = colliders;
            }

            return result;
        }

        /// <summary>
        /// Box 检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="size">尺寸</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult OverlapBox(FPVector3 position, FPQuaternion rotation, FPVector3 size, int layer = -1)
        {
            if (false == stage.SeekBehaviorInfos(out List<ColliderInfo> colliders)) return default;
            
            Box box = new Box
            {
                offset = FPVector3.zero,
                size = size
            };
            HitResult result = new();
            foreach (var collider in colliders)
            {
                if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) continue;
                if (-1 != layer && false == COLLIDER_DEFINE.QUERY(layer, collider.layer)) continue;
                
                bool hit = false;
                FPVector3 point = FPVector3.zero;
                FPVector3 normal = FPVector3.zero;
                FP penetration = FP.Zero;
                switch (collider.shape)
                {
                    case COLLIDER_DEFINE.BOX:
                        hit = Detect(box, collider.box, position, spatial.position, rotation, FPQuaternion.Euler(spatial.euler), out point, out normal, out penetration);
                        break;
                    case COLLIDER_DEFINE.SPHERE:
                        hit = Detect(box, collider.sphere, position, spatial.position, rotation, out point, out normal, out penetration);
                        break;
                }
                if (false == hit) continue;
                
                result.hit = hit;
                if (null == result.colliders) result.colliders = ObjectCache.Ensure<List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)>>();
                result.colliders.Add((collider.id, point, normal, penetration));
            }
            colliders.Clear();
            ObjectCache.Set(colliders);

            return result;
        }

        /// <summary>
        /// Sphere 检测
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult OverlapSphere(FPVector3 position, FP radius, int layer = -1)
        {
            if (false == stage.SeekBehaviorInfos(out List<ColliderInfo> colliders)) return default;
            
            Sphere sphere = new Sphere
            {
                offset = FPVector3.zero,
                radius = radius
            };
            HitResult result = new();
            foreach (var collider in colliders)
            {
                if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) continue;
                if (-1 != layer && false == COLLIDER_DEFINE.QUERY(layer, collider.layer)) continue;

                bool hit = false;
                FPVector3 point = FPVector3.zero;
                FPVector3 normal = FPVector3.zero;
                FP penetration = FP.Zero;
                switch (collider.shape)
                {
                    case COLLIDER_DEFINE.BOX:
                        hit = Detect(collider.box, sphere, spatial.position, position, FPQuaternion.Euler(spatial.euler), out point, out normal, out penetration);
                        break;
                    case COLLIDER_DEFINE.SPHERE:
                        hit = Detect(sphere, collider.sphere, position, spatial.position, out point, out normal, out penetration);
                        break;
                }
                if (false == hit) continue;

                result.hit = hit;
                if (null == result.colliders) result.colliders = ObjectCache.Ensure<List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)>>();
                result.colliders.Add((collider.id, point, normal, penetration));
            }
            colliders.Clear();
            ObjectCache.Set(colliders);
            

            return result;
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="origin">原点</param>
        /// <param name="dire">方向</param>
        /// <param name="distance">距离</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult Raycast(FPVector3 origin, FPVector3 dire, FP distance, int layer = -1)
        {
            if (false == stage.SeekBehaviorInfos(out List<ColliderInfo> colliders)) return default;

            HitResult result = new();
            foreach (var collider in colliders)
            {
                if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) continue;
                if (-1 != layer && false == COLLIDER_DEFINE.QUERY(layer, collider.layer)) continue;

                bool hit = false;
                FPVector3 point = FPVector3.zero;
                FPVector3 normal = FPVector3.zero;
                FP penetration = FP.Zero;
                switch (collider.shape)
                {
                    case COLLIDER_DEFINE.BOX:
                        hit = Raycast(origin, dire, distance, collider.box, spatial.position, FPQuaternion.Euler(spatial.euler), out point, out normal, out penetration);
                        break;
                    case COLLIDER_DEFINE.SPHERE:
                        hit = Raycast(origin, dire, distance, collider.sphere, spatial.position, out point, out normal, out penetration);
                        break;
                }
                if (false == hit) continue;
                
                result.hit = hit;
                if (null == result.colliders) result.colliders = ObjectCache.Ensure<List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)>>();
                result.colliders.Add((collider.id, point, normal, penetration));
            }
            colliders.Clear();
            ObjectCache.Set(colliders);
            
            return result;
        }

        /// <summary>
        /// 线段检测
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="layer">层级 (-1, 默认全检测)</param>
        /// <returns>结果</returns>
        public HitResult Linecast(FPVector3 start, FPVector3 end, int layer = -1)
        {
            // 计算射线方向和长度
            FPVector3 dire = end - start;
            return Raycast(start, dire.normalized, dire.magnitude, layer);
        }

        /// <summary>
        /// AABB 碰撞检测
        /// </summary>
        /// <param name="a">AABB-A</param>
        /// <param name="b">AABB-B</param>
        /// <returns>YES/NO</returns>
        public bool Detect(AABB a, AABB b)
        {
            FPVector3 min1 = a.position - a.size * FP.Half;
            FPVector3 max1 = a.position + a.size * FP.Half;
            FPVector3 min2 = b.position - b.size * FP.Half;
            FPVector3 max2 = b.position + b.size * FP.Half;

            return
            (
                min1.x < max2.x && max1.x > min2.x
                                &&
                min1.y < max2.y && max1.y > min2.y
                                &&
                min1.z < max2.z && max1.z > min2.z
            );
        }

        /// <summary>
        /// 检测两个 Box 之间的碰撞
        /// </summary>
        /// <param name="a">BoxA</param>
        /// <param name="b">BoxB</param>
        /// <param name="apos">BoxA 位置</param>
        /// <param name="bpos">BoxB 位置</param>
        /// <param name="arot">BoxA 旋转</param>
        /// <param name="brot">BoxB 旋转</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从 BoxB 指向 BoxA 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Detect(Box a, Box b, FPVector3 apos, FPVector3 bpos, FPQuaternion arot, FPQuaternion brot, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;
            
            apos += a.offset;
            bpos += b.offset;

            // 获取两个立方体的轴向向量
            var aaxes = GetAxes(arot);
            var baxes = GetAxes(brot);

            // 生成所有分离轴（15个轴）
            var axes = ObjectCache.Ensure<List<FPVector3>>();
            axes.AddRange(aaxes);
            axes.AddRange(baxes);

            for (int i = 0; i < aaxes.Count; i++)
            {
                for (int j = 0; j < baxes.Count; j++)
                {
                    FPVector3 crossProduct = FPVector3.Cross(aaxes[i], baxes[j]);
                    if (crossProduct.sqrMagnitude > FP.Epsilon)
                    {
                        axes.Add(crossProduct.normalized);
                    }
                }
            }
            aaxes.Clear();
            baxes.Clear();
            ObjectCache.Set(aaxes);
            ObjectCache.Set(baxes);

            // 遍历每个轴，检查是否存在分离
            foreach (var axis in axes)
            {
                // 投影两个立方体到当前轴上
                (FP min1, FP max1) = ProjectBoxOntoAxis(a, apos, arot, axis);
                (FP min2, FP max2) = ProjectBoxOntoAxis(b, bpos, brot, axis);

                // 计算投影的重叠量
                FP overlap = GetOverlap(min1, max1, min2, max2);

                // 检查是否有分离轴
                if (overlap <= 0)
                {
                    axes.Clear();
                    ObjectCache.Set(axes);
                    
                    // 存在分离轴，表示无碰撞
                    return false;
                }

                // 如果有碰撞，找到最小重叠量的轴
                if (overlap < penetration)
                {
                    penetration = overlap;
                    normal = axis;
                }
            }
            axes.Clear();
            ObjectCache.Set(axes);
            
            if (FPVector3.Dot(normal, bpos - apos) > 0)
            {
                normal = -normal;
            }

            // 若所有轴都重叠，则发生碰撞
            // 计算碰撞点（将使用穿透最小的轴进行计算）
            point = CalculateCollisionPoint(apos, bpos, normal, penetration);

            return true;
        }

        /// <summary>
        /// 检测两个球体之间的碰撞
        /// </summary>
        /// <param name="a">SphereA</param>
        /// <param name="b">SphereB</param>
        /// <param name="apos">SphereA 位置</param>
        /// <param name="bpos">SphereB 位置</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从 SphereB 指向 SphereA 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Detect(Sphere a, Sphere b, FPVector3 apos, FPVector3 bpos, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;
            apos += a.offset;
            bpos += b.offset;

            // 计算两个球体之间的距离
            FP distance = FPVector3.Distance(apos, bpos);
            FP radius1 = a.radius;
            FP radius2 = b.radius;

            // 检查是否有碰撞
            if (distance > radius1 + radius2)
            {
                return false;
            }

            // 计算碰撞法线
            normal = (apos - bpos).normalized;

            // 计算碰撞点
            point = bpos + normal * radius2;

            // 计算穿透深度
            penetration = radius1 + radius2 - distance;

            return true;
        }
        
        /// <summary>
        /// 检测 Box 和 Sphere 之间的碰撞
        /// </summary>
        /// <param name="a">Box</param>
        /// <param name="b">Sphere</param>
        /// <param name="apos">Box 位置</param>
        /// <param name="bpos">Sphere 位置</param>
        /// <param name="arot">Box 旋转</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从 Box 指向 Sphere 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Detect(Box a, Sphere b, FPVector3 apos, FPVector3 bpos, FPQuaternion arot, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue; // 初始值设为最大
            apos += a.offset;
            bpos += b.offset;

            // 获取 BoxShape 的局部坐标轴（右、上、前）并返回它们在世界坐标中的方向
            var axes = GetAxes(arot);
            bool hasCollision = false;

            // 存储用于确定最近碰撞点和最小穿透的变量
            FP minPenetration = FP.MaxValue;
            FPVector3 bestAxis = FPVector3.zero;

            for (int i = 0; i < axes.Count; i++)
            {
                // 投影 BoxShape 到当前轴上
                (FP min1, FP max1) = ProjectBoxOntoAxis(a, apos, arot, axes[i]);
                FP radius = b.radius;
                FP min2 = FPVector3.Dot(bpos, axes[i]) - radius;
                FP max2 = FPVector3.Dot(bpos, axes[i]) + radius;

                // 计算投影的重叠量
                FP overlap = GetOverlap(min1, max1, min2, max2);

                // 检查是否有分离轴
                if (overlap <= 0)
                {
                    // 如果存在分离轴，表示无碰撞
                    return false;
                }

                // 找到最小重叠量的轴来作为潜在的碰撞法线
                if (overlap < minPenetration)
                {
                    minPenetration = overlap;
                    bestAxis = axes[i];
                    hasCollision = true;
                }
            }
            axes.Clear();
            ObjectCache.Set(axes);

            if (hasCollision)
            {
                // 确保法线方向指向球体方向
                FPVector3 direction = bpos - apos;
                normal = (FPVector3.Dot(bestAxis, direction) < 0) ? -bestAxis : bestAxis;

                // 计算碰撞点（基于球体表面上的最近点）
                point = bpos - normal * b.radius;

                // 将最小重叠量设为穿透量
                penetration = minPenetration;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Box 线段检测
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <param name="box">Box</param>
        /// <param name="position">Box 位置</param>
        /// <param name="rotation">Box 旋转</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">线段接触 Box 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Linecast(FPVector3 start, FPVector3 end, Box box, FPVector3 position, FPQuaternion rotation, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            // 计算射线方向和长度
            FPVector3 dire = end - start;
            // 调用射线检测方法
            return Raycast(start, dire.normalized, dire.magnitude, box, position, rotation, out point, out normal, out penetration);
        }

        /// <summary>
        /// Box 线段检测
        /// </summary>
        /// <param name="start">线段起点</param>
        /// <param name="end">线段终点</param>
        /// <param name="sphere">Sphere</param>
        /// <param name="position">Sphere 位置</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">线段接触 Sphere 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Linecast(FPVector3 start, FPVector3 end, Sphere sphere, FPVector3 position, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            // 计算射线方向和长度
            FPVector3 dire = end - start;
            // 调用射线检测方法
            return Raycast(start, dire.normalized, dire.magnitude, sphere, position, out point, out normal, out penetration);
        }

        /// <summary>
        /// Box 射线检测
        /// </summary>
        /// <param name="origin">射线起点</param>
        /// <param name="dire">射线方向</param>
        /// <param name="distance">射线距离</param>
        /// <param name="box">Box</param>
        /// <param name="position">Box 位置</param>
        /// <param name="rotation">Box 旋转</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">射中 Box 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public bool Raycast(FPVector3 origin, FPVector3 dire, FP distance, Box box, FPVector3 position, FPQuaternion rotation, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 修正盒子的实际位置，加入 center 属性
            FPVector3 boxCenter = position + box.offset;

            var axes = GetAxes(rotation);
            FPVector3 halfSize = box.size * FP.Half;

            FP minDistance = FP.MaxValue;
            bool hit = false;

            for (int i = 0; i < 3; i++)
            {
                FPVector3 axis = axes[i];
                FP halfSizeComponent = i == 0 ? halfSize.x : (i == 1 ? halfSize.y : halfSize.z);

                for (int j = -1; j <= 1; j += 2)
                {
                    // 修正平面位置计算
                    FPVector3 planePoint = boxCenter + axis * halfSizeComponent * j;
                    FP d = FPVector3.Dot(planePoint - origin, axis) / FPVector3.Dot(dire, axis);

                    if (d >= 0 && d <= distance)
                    {
                        FPVector3 hitPoint = origin + dire * d;
                        bool inside = true;

                        for (int k = 0; k < 3; k++)
                        {
                            if (k == i) continue;
                            FPVector3 otherAxis = axes[k];
                            FP extent = k == 0 ? halfSize.x : (k == 1 ? halfSize.y : halfSize.z);
                            FP projection = FPVector3.Dot(hitPoint - boxCenter, otherAxis);

                            if (FP.Abs(projection) > extent)
                            {
                                inside = false;
                                break;
                            }
                        }

                        if (inside && d < minDistance)
                        {
                            minDistance = d;
                            point = hitPoint;
                            normal = axis * j;
                            hit = true;
                        }
                    }
                }
            }
            axes.Clear();
            ObjectCache.Set(axes);

            if (hit)
            {
                penetration = distance - minDistance;
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Sphere 射线检测
        /// </summary>
        /// <param name="origin">射线起点</param>
        /// <param name="dire">射线方向</param>
        /// <param name="distance">射线距离</param>
        /// <param name="sphere">Sphere</param>
        /// <param name="position">Sphere 位置</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">射中 Sphere 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public static bool Raycast(FPVector3 origin, FPVector3 dire, FP distance, Sphere sphere, FPVector3 position, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 修正球体中心点，加入 center 属性
            FPVector3 sphereCenter = position + sphere.offset;

            // 计算射线与球体中心的方向
            FPVector3 direction = sphereCenter - origin;
            FP projection = FPVector3.Dot(direction, dire);

            // 如果射线的投影小于 0，表示射线背向球体
            if (projection < FP.Zero)
                return false;

            // 计算射线上与球体中心最近的点
            FPVector3 pointOnRay = origin + dire * projection;

            // 检查射线与球体的距离是否在允许范围内
            FP currentDistance = FPVector3.Distance(pointOnRay, sphereCenter);
            if (currentDistance > sphere.radius || projection > distance)
                return false;

            // 计算碰撞点、法线和穿透深度
            point = pointOnRay;
            normal = (pointOnRay - sphereCenter).normalized;
            penetration = sphere.radius - currentDistance;

            return true;
        }

        /// <summary>
        /// 设置碰撞盒信息
        /// </summary>
        /// <param name="collider">碰撞盒</param>
        /// <param name="colliderid">碰撞盒配置 ID</param>
        public void SetColliderInfo(ColliderInfo collider, int id)
        {
            if (false == stage.cfg.location.ColliderInfos.TryGetValue(id, out var collidercfg)) return;
            if (null == collidercfg) return;
            
            switch (collidercfg.Type)
            {
                case COLLIDER_DEFINE.BOX:
                    collider.shape = COLLIDER_DEFINE.BOX;
                    collider.box = new Box
                    {
                        offset = new FPVector3
                        (
                            collidercfg.Offset[0] * stage.cfg.int2fp,
                            collidercfg.Offset[1] * stage.cfg.int2fp,
                            collidercfg.Offset[2] * stage.cfg.int2fp
                        ),
                        size = new FPVector3(
                            collidercfg.Shape[0] * FP.Half * stage.cfg.int2fp,
                            collidercfg.Shape[1] * FP.Half * stage.cfg.int2fp,
                            collidercfg.Shape[2] * FP.Half * stage.cfg.int2fp
                        )
                    };
                    break;
                case COLLIDER_DEFINE.SPHERE:
                    collider.shape = COLLIDER_DEFINE.SPHERE;
                    collider.sphere = new Sphere
                    {
                        offset = new FPVector3
                        (
                            collidercfg.Offset[0] * stage.cfg.int2fp,
                            collidercfg.Offset[1] * stage.cfg.int2fp,
                            collidercfg.Offset[2] * stage.cfg.int2fp
                        ),
                        radius = collidercfg.Shape[0] * stage.cfg.int2fp
                    };
                    break;
            }
        }

        /// <summary>
        /// 计算 AABB 包围盒
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="aabb">AABB 包围盒</param>
        /// <returns>YES/NO</returns>
        public bool CalcAABB(ulong id, out AABB aabb)
        {
            aabb = default;
            if (false == stage.SeekBehaviorInfo(id, out ColliderInfo collider)) return false;

            return CalcAABB(collider, out aabb);
        }

        /// <summary>
        /// 计算 AABB 包围盒
        /// </summary>
        /// <param name="collider">碰撞盒信息</param>
        /// <param name="aabb">AABB 包围盒</param>
        /// <returns>YES/NO</returns>
        public bool CalcAABB(ColliderInfo collider,  out AABB aabb)
        {
            aabb = default;
            if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) return false;

            FPVector3 position = spatial.position;
            FPQuaternion rotation = FPQuaternion.Euler(spatial.euler);
            switch (collider.shape)
            {
                case COLLIDER_DEFINE.BOX:
                    // 获取 BoxShape 的半尺寸
                    FPVector3 halfSize = collider.box.size * FP.Half;

                    // 定义 box 的 8 个顶点（以中心为基准）
                    var vertices = ObjectCache.Ensure<List<FPVector3>>();
                    vertices.Add(new FPVector3(-halfSize.x, -halfSize.y, -halfSize.z));
                    vertices.Add(new FPVector3(halfSize.x, -halfSize.y, -halfSize.z));
                    vertices.Add(new FPVector3(-halfSize.x, halfSize.y, -halfSize.z));
                    vertices.Add(new FPVector3(halfSize.x, halfSize.y, -halfSize.z));
                    vertices.Add(new FPVector3(-halfSize.x, -halfSize.y, halfSize.z));
                    vertices.Add(new FPVector3(halfSize.x, -halfSize.y, halfSize.z));
                    vertices.Add(new FPVector3(-halfSize.x, halfSize.y, halfSize.z));
                    vertices.Add(new FPVector3(halfSize.x, halfSize.y, halfSize.z));
                    

                    // 初始化 AABB 的最小值和最大值
                    FPVector3 min = FPVector3.MaxValue;
                    FPVector3 max = FPVector3.MinValue;

                    // 对每个顶点应用旋转和位移，更新 min 和 max
                    foreach (var vertex in vertices)
                    {
                        FPVector3 transformedVertex = rotation * (vertex + collider.box.offset);
                        min = FPVector3.Min(min, transformedVertex);
                        max = FPVector3.Max(max, transformedVertex);
                    }
                    vertices.Clear();
                    ObjectCache.Set(vertices);

                    // 计算最终的 AABB
                    aabb = new AABB
                    {
                        position = position + (min + max) * FP.Half,
                        size = max - min
                    };
                    
                    return true;
                case COLLIDER_DEFINE.SPHERE:
                    // Sphere 不受旋转影响
                    aabb = new AABB
                    {
                        position = position + rotation * collider.sphere.offset,
                        size = new FPVector3(collider.sphere.radius * 2)
                    };
                    
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// 获取 Box 的局部坐标轴（右、上、前）并返回它们在世界坐标中的方向
        /// </summary>
        /// <param name="rotation">旋转</param>
        /// <returns>局部坐标轴（右、上、前）</returns>
        private static List<FPVector3> GetAxes(FPQuaternion rotation)
        {
            var vertices = ObjectCache.Ensure<List<FPVector3>>();
            vertices.Add(rotation * FPVector3.right);
            vertices.Add(rotation * FPVector3.up);
            vertices.Add(rotation * FPVector3.forward);

            return vertices;
        }
        
        /// <summary>
        /// 投影立方体的 8 个顶点到轴上，找到投影的最小值和最大值
        /// </summary>
        /// <param name="box">Box</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="axis">轴</param>
        /// <returns>(最小, 最大)</returns>
        private (FP min, FP max) ProjectBoxOntoAxis(Box box, FPVector3 position, FPQuaternion rotation, FPVector3 axis)
        {
            var vertices = GetBoxVertices(box, position, rotation);
            FP min = FPVector3.Dot(vertices[0], axis);
            FP max = min;

            for (int i = 1; i < vertices.Count; i++)
            {
                FP projection = FPVector3.Dot(vertices[i], axis);
                min = FP.Min(min, projection);
                max = FP.Max(max, projection);
            }
            vertices.Clear();
            ObjectCache.Set(vertices);

            return (min, max);
        }
        
        /// <summary>
        /// 计算立方体的 8 个顶点位置
        /// </summary>
        /// <param name="box">Box</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>8 个顶点位置</returns>
        private List<FPVector3> GetBoxVertices(Box box, FPVector3 position, FPQuaternion rotation)
        {
            var vertices = ObjectCache.Ensure<List<FPVector3>>();
            FPVector3 extents = box.size * FP.Half;

            // 生成相对于中心的8个顶点
            vertices.Add(position + rotation * new FPVector3(-extents.x, -extents.y, -extents.z));
            vertices.Add(position + rotation * new FPVector3(extents.x, -extents.y, -extents.z));
            vertices.Add(position + rotation * new FPVector3(extents.x, extents.y, -extents.z));
            vertices.Add(position + rotation * new FPVector3(-extents.x, extents.y, -extents.z));
            vertices.Add(position + rotation * new FPVector3(-extents.x, -extents.y, extents.z));
            vertices.Add(position + rotation * new FPVector3(extents.x, -extents.y, extents.z));
            vertices.Add(position + rotation * new FPVector3(extents.x, extents.y, extents.z));
            vertices.Add(position + rotation * new FPVector3(-extents.x, extents.y, extents.z));

            return vertices;
        }
        
        private FP GetOverlap(FP amin, FP amax, FP bmin, FP bmax)
        {
            // 添加 epsilon 来处理精度问题
            FP overlap = FP.Min(amax, bmax) - FP.Max(amin, bmin);

            return (overlap < FP.Epsilon) ? FP.Zero : overlap;
        }
        
        private FPVector3 CalculateCollisionPoint(FPVector3 apos, FPVector3 bpos, FPVector3 collisionAxis, FP penetration)
        {
            // 计算沿着碰撞法线轴的移动方向
            FPVector3 moveDirection = collisionAxis.normalized;

            // 根据穿透深度计算每个立方体沿着法线轴的移动量
            FP halfPenetration = penetration * FP.Half;

            // 计算碰撞后的调整位置
            FPVector3 newPosition1 = apos - moveDirection * halfPenetration;
            FPVector3 newPosition2 = bpos + moveDirection * halfPenetration;

            // 返回调整后的碰撞点，避免过度偏移
            return (newPosition1 + newPosition2) * FP.Half;
        }
    }
}