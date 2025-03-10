using System.Collections.Concurrent;
using Kowtow.Collision.Shapes;
using Kowtow.Math;
using System.Collections.Generic;

namespace Kowtow.Collision
{
    /// <summary>
    /// 碰撞检测
    /// </summary>
    public class Detection
    {
        /// <summary>
        /// 判断 AABB 2 是否包含另一个 AABB 1
        /// </summary>
        /// <param name="aabb1">AABB 1</param>
        /// <param name="aabb2">AABB 2</param>
        /// <returns>YES/NO</returns>
        public static bool InsideAABB(AABB aabb1, AABB aabb2)
        {
            FPVector3 min1 = aabb2.center - aabb2.size * FP.Half;
            FPVector3 max1 = aabb2.center + aabb2.size * FP.Half;
            FPVector3 min2 = aabb1.center - aabb1.size * FP.Half;
            FPVector3 max2 = aabb1.center + aabb1.size * FP.Half;

            return (
                min1.x <= min2.x && min1.y <= min2.y && min1.z <= min2.z &&
                max1.x >= max2.x && max1.y >= max2.y && max1.z >= max2.z
            );
        }

        /// <summary>
        /// AABB 碰撞检测
        /// </summary>
        /// <param name="aabb1">AABB 1</param>
        /// <param name="aabb2">AABB 2</param>
        /// <returns>YES/NO</returns>
        public static bool DetectAABB(AABB aabb1, AABB aabb2)
        {
            FPVector3 min1 = aabb1.center - aabb1.size * FP.Half;
            FPVector3 max1 = aabb1.center + aabb1.size * FP.Half;
            FPVector3 min2 = aabb2.center - aabb2.size * FP.Half;
            FPVector3 max2 = aabb2.center + aabb2.size * FP.Half;

            return (
                min1.x < max2.x && max1.x > min2.x &&
                min1.y < max2.y && max1.y > min2.y &&
                min1.z < max2.z && max1.z > min2.z
            );
        }

        /// <summary>
        /// 检测两个 AABB 是否在给定运动路径上相交（考虑旋转）
        /// </summary>
        /// <param name="a">AABB A</param>
        /// <param name="b">AABB B</param>
        /// <param name="startA">A 的起始位置</param>
        /// <param name="endA">A 的结束位置</param>
        /// <param name="startRotA">A 的起始旋转</param>
        /// <param name="endRotA">A 的结束旋转</param>
        /// <param name="startB">B 的起始位置</param>
        /// <param name="endB">B 的结束位置</param>
        /// <param name="startRotB">B 的起始旋转</param>
        /// <param name="endRotB">B 的结束旋转</param>
        /// <returns>YES/NO</returns>
        public static bool SweepAABB(AABB a, AABB b, FPVector3 startA, FPVector3 endA, FPQuaternion startRotA, FPQuaternion endRotA, FPVector3 startB, FPVector3 endB, FPQuaternion startRotB, FPQuaternion endRotB)
        {
            // 计算扩展的 AABB（考虑旋转）
            AABB expandedA = ExpandedAABB(a, startA, endA, startRotA, endRotA);
            AABB expandedB = ExpandedAABB(b, startB, endB, startRotB, endRotB);

            return DetectAABB(expandedA, expandedB);
        }

        /// <summary>
        /// 计算扩展的 AABB，以覆盖刚体的运动路径（同时考虑位置和旋转的变化）
        /// </summary>
        /// <param name="aabb">原始 AABB</param>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="startRot">起始旋转</param>
        /// <param name="endRot">结束旋转</param>
        /// <returns>扩展后的 AABB</returns>
        private static AABB ExpandedAABB(AABB aabb, FPVector3 start, FPVector3 end, FPQuaternion startRot, FPQuaternion endRot)
        {
            // 计算起始和结束旋转作用后的 AABB
            AABB startAABB = RotateAABB(aabb, startRot);
            startAABB.center += start; // 将起始 AABB 移动到起始位置

            AABB endAABB = RotateAABB(aabb, endRot);
            endAABB.center += end; // 将结束 AABB 移动到结束位置

            // 计算包围盒的最小点和最大点，综合考虑位置和旋转
            FPVector3 min = FPVector3.Min(startAABB.center - startAABB.size * FP.Half, endAABB.center - endAABB.size * FP.Half);
            FPVector3 max = FPVector3.Max(startAABB.center + startAABB.size * FP.Half, endAABB.center + endAABB.size * FP.Half);

            // 返回扩展后的 AABB
            return new AABB
            {
                center = (min + max) * FP.Half,
                size = max - min
            };
        }

        /// <summary>
        /// 根据旋转调整 AABB 的范围
        /// </summary>
        /// <param name="aabb">原始 AABB</param>
        /// <param name="rotation">旋转</param>
        /// <returns>调整后的 AABB</returns>
        private static AABB RotateAABB(AABB aabb, FPQuaternion rotation)
        {
            // 获取 AABB 的 8 个顶点
            FPVector3 halfSize = aabb.size * FP.Half;
            
            var vertices = ObjectPool.GetVertices(8);
            vertices[0] = new(-halfSize.x, -halfSize.y, -halfSize.z);
            vertices[1] = new(halfSize.x, -halfSize.y, -halfSize.z);
            vertices[2] = new(-halfSize.x, halfSize.y, -halfSize.z);
            vertices[3] = new(halfSize.x, halfSize.y, -halfSize.z);
            vertices[4] = new(-halfSize.x, -halfSize.y, halfSize.z);
            vertices[5] = new(halfSize.x, -halfSize.y, halfSize.z);
            vertices[6] = new(-halfSize.x, halfSize.y, halfSize.z);
            vertices[7] = new(halfSize.x, halfSize.y, halfSize.z);

            // 应用旋转，更新顶点的范围
            FPVector3 min = FPVector3.MaxValue;
            FPVector3 max = FPVector3.MinValue;

            foreach (var vertex in vertices)
            {
                FPVector3 transformedVertex = rotation * vertex;
                min = FPVector3.Min(min, transformedVertex);
                max = FPVector3.Max(max, transformedVertex);
            }
            ObjectPool.SetVertices(vertices);

            return new AABB
            {
                center = (min + max) * FP.Half,
                size = max - min
            };
        }

        /// <summary>
        /// 检测两个 Rigidbody 是否在给定运动路径和旋转变化范围内发生碰撞，并返回碰撞时间点 TOI（Time of Impact）
        /// </summary>
        /// <param name="a">Rigidbody A</param>
        /// <param name="b">Rigidbody B</param>
        /// <param name="startA">A 的起始位置</param>
        /// <param name="endA">A 的结束位置</param>
        /// <param name="startB">B 的起始位置</param>
        /// <param name="endB">B 的结束位置</param>
        /// <param name="startRotA">A 的起始旋转</param>
        /// <param name="endRotA">A 的结束旋转</param>
        /// <param name="startRotB">B 的起始旋转</param>
        /// <param name="endRotB">B 的结束旋转</param>
        /// <param name="toi">碰撞时间点（归一化时间，0 到 1 范围内）</param>
        /// <returns>YES/NO</returns>
        public static bool Sweep(Rigidbody a, Rigidbody b, FPVector3 startA, FPVector3 endA, FPVector3 startB, FPVector3 endB, FPQuaternion startRotA, FPQuaternion endRotA, FPQuaternion startRotB, FPQuaternion endRotB, out FP toi)
        {
            toi = FP.One;
            // 初步检测两个刚体的扩展 AABB 是否相交，若无相交直接返回 false
            if (false == SweepAABB(a.aabb, b.aabb, startA, endA, startRotA, endRotA, startB, endB, startRotB, endRotB)) return false;

            FP step = FP.Zero;
            while (true)
            {
                step += FP.Epsilon;
                if (step > FP.One) break;

                FPVector3 positionA = FPVector3.Lerp(startA, endA, step);
                FPVector3 positionB = FPVector3.Lerp(startB, endB, step);
                FPQuaternion rotationA = FPQuaternion.Slerp(startRotA, endRotA, step);
                FPQuaternion rotationB = FPQuaternion.Slerp(startRotB, endRotB, step);

                if (Detect(a.shape, b.shape, positionA, positionB, rotationA, rotationB, out _, out _, out _, false))
                {
                    toi = step;
                    break;
                }
            }

            return toi < FP.One;
        }

        /// <summary>
        /// 刚体与刚体碰撞检测
        /// </summary>
        /// <param name="rigidbody1">刚体 1</param>
        /// <param name="rigidbody2">刚体 2</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从刚体 2 指向刚体 1 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public static bool Detect(Rigidbody rigidbody1, Rigidbody rigidbody2, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            if (false == DetectAABB(rigidbody1.aabb, rigidbody2.aabb)) return false;

            return Detect(rigidbody1.shape, rigidbody2.shape, rigidbody1.position, rigidbody2.position, rigidbody1.rotation, rigidbody2.rotation, out point, out normal, out penetration, false);
        }

        /// <summary>
        /// 几何体与几何体碰撞检测
        /// </summary>
        /// <param name="shape1">几何体 1</param>
        /// <param name="shape2">几何体 2</param>
        /// <param name="position1">坐标 1</param>
        /// <param name="position2">坐标 2</param>
        /// <param name="rotation1">旋转 1</param>
        /// <param name="rotation2">旋转 2</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从几何体 2 指向几何体 1 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>YES/NO</returns>
        public static bool Detect(IShape shape1, IShape shape2, FPVector3 position1, FPVector3 position2, FPQuaternion rotation1, FPQuaternion rotation2, out FPVector3 point, out FPVector3 normal, out FP penetration, bool aabbdetect = true)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // AABB 检测
            if (aabbdetect)
            {
                var aabb1 = AABB.CreateFromShape(shape1, position1, rotation1);
                var aabb2 = AABB.CreateFromShape(shape2, position2, rotation2);

                if (false == DetectAABB(aabb1, aabb2)) return false;
            }

            position1 += shape1.center;
            position2 += shape2.center;

            if (shape1 is BoxShape && shape2 is BoxShape)
            {
                return DetectBoxBox(shape1 as BoxShape, shape2 as BoxShape, position1, position2, rotation1, rotation2, out point, out normal, out penetration);
            }
            else if (shape1 is BoxShape && shape2 is SphereShape)
            {
                return DetectBoxSphere(shape1 as BoxShape, shape2 as SphereShape, position1, position2, rotation1, out point, out normal, out penetration);
            }
            else if (shape1 is SphereShape && shape2 is SphereShape)
            {
                return DetectSphereSphere(shape1 as SphereShape, shape2 as SphereShape, position1, position2, out point, out normal, out penetration);
            }
            else if (shape1 is SphereShape && shape2 is BoxShape)
            {
                return DetectBoxSphere(shape2 as BoxShape, shape1 as SphereShape, position2, position1, rotation2, out point, out normal, out penetration);
            }

            return false;
        }

        /// <summary>
        /// 检测两个 BoxShape 之间的碰撞
        /// </summary>
        /// <param name="box1">立方体 1</param>
        /// <param name="box2">立方体 2</param>
        /// <param name="point">碰撞点</param>
        /// <param name="normal">从立方体 2 指向立方体 1 的法线</param>
        /// <param name="penetration">穿透深度</param>
        /// <returns>是否发生碰撞</returns>
        private static bool DetectBoxBox(BoxShape box1, BoxShape box2, FPVector3 position1, FPVector3 position2, FPQuaternion rotation1, FPQuaternion rotation2, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 获取两个立方体的轴向向量
            FPVector3[] axes1 = GetAxes(rotation1);
            FPVector3[] axes2 = GetAxes(rotation2);

            // 生成所有分离轴（15个轴）
            var axes = ObjectPool.GetVerticesList();
            axes.AddRange(axes1);
            axes.AddRange(axes2);

            for (int i = 0; i < axes1.Length; i++)
            {
                for (int j = 0; j < axes2.Length; j++)
                {
                    FPVector3 crossProduct = FPVector3.Cross(axes1[i], axes2[j]);
                    if (crossProduct.sqrMagnitude > FP.Epsilon)
                    {
                        axes.Add(crossProduct.normalized);
                    }
                }
            }
            ObjectPool.SetVertices(axes1);
            ObjectPool.SetVertices(axes2);

            // 遍历每个轴，检查是否存在分离
            foreach (var axis in axes)
            {
                // 投影两个立方体到当前轴上
                (FP min1, FP max1) = ProjectBoxOntoAxis(box1, position1, rotation1, axis);
                (FP min2, FP max2) = ProjectBoxOntoAxis(box2, position2, rotation2, axis);

                // 计算投影的重叠量
                FP overlap = GetOverlap(min1, max1, min2, max2);

                // 检查是否有分离轴
                if (overlap <= 0)
                {
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
            ObjectPool.SetVerticesList(axes);
            
            if (FPVector3.Dot(normal, position2 - position1) > 0)
            {
                normal = -normal;
            }

            // 若所有轴都重叠，则发生碰撞
            // 计算碰撞点（将使用穿透最小的轴进行计算）
            point = CalculateCollisionPoint(position1, position2, normal, penetration);

            return true;
        }

        private static bool DetectBoxSphere(BoxShape box, SphereShape sphere, FPVector3 position1, FPVector3 position2, FPQuaternion rotation1, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue; // 初始值设为最大

            // 获取 BoxShape 的局部坐标轴（右、上、前）并返回它们在世界坐标中的方向
            FPVector3[] axes = GetAxes(rotation1);
            bool hasCollision = false;

            // 存储用于确定最近碰撞点和最小穿透的变量
            FP minPenetration = FP.MaxValue;
            FPVector3 bestAxis = FPVector3.zero;

            for (int i = 0; i < axes.Length; i++)
            {
                // 投影 BoxShape 到当前轴上
                (FP min1, FP max1) = ProjectBoxOntoAxis(box, position1, rotation1, axes[i]);
                FP radius = sphere.radius;
                FP min2 = FPVector3.Dot(position2, axes[i]) - radius;
                FP max2 = FPVector3.Dot(position2, axes[i]) + radius;

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
            ObjectPool.SetVertices(axes);

            if (hasCollision)
            {
                // 确保法线方向指向球体方向
                FPVector3 direction = position2 - position1;
                normal = (FPVector3.Dot(bestAxis, direction) < 0) ? -bestAxis : bestAxis;

                // 计算碰撞点（基于球体表面上的最近点）
                point = position2 - normal * sphere.radius;

                // 将最小重叠量设为穿透量
                penetration = minPenetration;

                return true;
            }

            return false;
        }

        private static bool DetectSphereSphere(SphereShape sphere1, SphereShape sphere2, FPVector3 position1, FPVector3 position2, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 计算两个球体之间的距离
            FP distance = FPVector3.Distance(position1, position2);
            FP radius1 = sphere1.radius;
            FP radius2 = sphere2.radius;

            // 检查是否有碰撞
            if (distance > radius1 + radius2)
            {
                return false;
            }

            // 计算碰撞法线
            normal = (position1 - position2).normalized;

            // 计算碰撞点
            point = position2 + normal * radius2;

            // 计算穿透深度
            penetration = radius1 + radius2 - distance;

            return true;
        }

        public static bool DetectLineShape(FPVector3 start, FPVector3 end, IShape shape, FPVector3 position, FPQuaternion rotation, out FPVector3 point, out FPVector3 normal, out FP penetration, bool aabbdetect = true)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            FPVector3 direction = end - start;
            FP distance = direction.magnitude;
            FPVector3 ray = direction.normalized;

            if (aabbdetect)
            {
                FPVector3 min = FPVector3.Min(start, end);
                FPVector3 max = FPVector3.Max(start, end);
                AABB aabb = new AABB()
                {
                    center = (min + max) * FP.Half,
                    size = max - min
                };
                if (false == DetectAABB(aabb, AABB.CreateFromShape(shape, position, rotation))) return false;
            }

            if (shape is BoxShape)
            {
                return DetectBoxRay(shape as BoxShape, position, rotation, start, ray, distance, out point, out normal, out penetration);
            }
            else if (shape is SphereShape)
            {
                return DetectSphereRay(shape as SphereShape, position, start, ray, distance, out point, out normal, out penetration);
            }

            return false;
        }

        private static bool DetectBoxRay(BoxShape box, FPVector3 position, FPQuaternion rotation, FPVector3 start, FPVector3 ray, FP distance, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 修正盒子的实际位置，加入 center 属性
            FPVector3 boxCenter = position + box.center;

            FPVector3[] axes = GetAxes(rotation);
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
                    FP d = FPVector3.Dot(planePoint - start, axis) / FPVector3.Dot(ray, axis);

                    if (d >= 0 && d <= distance)
                    {
                        FPVector3 hitPoint = start + ray * d;
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
            ObjectPool.SetVertices(axes);

            if (hit)
            {
                penetration = distance - minDistance;
                return true;
            }

            return false;
        }


        private static bool DetectSphereRay(SphereShape sphere, FPVector3 position, FPVector3 start, FPVector3 ray, FP distance, out FPVector3 point, out FPVector3 normal, out FP penetration)
        {
            point = FPVector3.zero;
            normal = FPVector3.zero;
            penetration = FP.MaxValue;

            // 修正球体中心点，加入 center 属性
            FPVector3 sphereCenter = position + sphere.center;

            // 计算射线与球体中心的方向
            FPVector3 direction = sphereCenter - start;
            FP projection = FPVector3.Dot(direction, ray);

            // 如果射线的投影小于 0，表示射线背向球体
            if (projection < FP.Zero)
                return false;

            // 计算射线上与球体中心最近的点
            FPVector3 pointOnRay = start + ray * projection;

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

        private static FPVector3 ClosestPointOnLine(FPVector3 start, FPVector3 end, FPVector3 point)
        {
            FPVector3 lineDirection = (end - start).normalized;
            FP projection = FPVector3.Dot(point - start, lineDirection);
            projection = FP.Clamp(projection, 0, FPVector3.Distance(start, end)); // 确保投影点在轴线范围内
            return start + lineDirection * projection;
        }

        private static FPVector3[] GetAxes(FPQuaternion rotation)
        {
            // 获取 BoxShape 的局部坐标轴（右、上、前）并返回它们在世界坐标中的方向
            var vertices = ObjectPool.GetVertices(3);
            vertices[0] = rotation * FPVector3.right;
            vertices[1] = rotation * FPVector3.up;
            vertices[2] = rotation * FPVector3.forward;

            return vertices;
        }

        private static (FP min, FP max) ProjectBoxOntoAxis(BoxShape box, FPVector3 position, FPQuaternion rotation, FPVector3 axis)
        {
            // 投影立方体的 8 个顶点到轴上，找到投影的最小值和最大值
            FPVector3[] vertices = GetBoxVertices(box, position, rotation);
            FP min = FPVector3.Dot(vertices[0], axis);
            FP max = min;

            for (int i = 1; i < vertices.Length; i++)
            {
                FP projection = FPVector3.Dot(vertices[i], axis);
                min = FP.Min(min, projection);
                max = FP.Max(max, projection);
            }
            ObjectPool.SetVertices(vertices);

            return (min, max);
        }

        private static FPVector3[] GetBoxVertices(BoxShape box, FPVector3 position, FPQuaternion rotation)
        {
            var vertices = ObjectPool.GetVertices(8);
            // 计算立方体的 8 个顶点位置
            FPVector3 extents = box.size * FP.Half;

            // 生成相对于中心的8个顶点
            vertices[0] = position + rotation * new FPVector3(-extents.x, -extents.y, -extents.z);
            vertices[1] = position + rotation * new FPVector3(extents.x, -extents.y, -extents.z);
            vertices[2] = position + rotation * new FPVector3(extents.x, extents.y, -extents.z);
            vertices[3] = position + rotation * new FPVector3(-extents.x, extents.y, -extents.z);
            vertices[4] = position + rotation * new FPVector3(-extents.x, -extents.y, extents.z);
            vertices[5] = position + rotation * new FPVector3(extents.x, -extents.y, extents.z);
            vertices[6] = position + rotation * new FPVector3(extents.x, extents.y, extents.z);
            vertices[7] = position + rotation * new FPVector3(-extents.x, extents.y, extents.z);

            return vertices;
        }

        private static FP GetOverlap(FP min1, FP max1, FP min2, FP max2)
        {
            // 添加 epsilon 来处理精度问题
            FP overlap = FP.Min(max1, max2) - FP.Max(min1, min2);

            return (overlap < FP.Epsilon) ? FP.Zero : overlap;
        }

        private static FPVector3 CalculateCollisionPoint(FPVector3 position1, FPVector3 position2, FPVector3 collisionAxis, FP penetration)
        {
            // 计算沿着碰撞法线轴的移动方向
            FPVector3 moveDirection = collisionAxis.normalized;

            // 根据穿透深度计算每个立方体沿着法线轴的移动量
            FP halfPenetration = penetration * FP.Half;

            // 计算碰撞后的调整位置
            FPVector3 newPosition1 = position1 - moveDirection * halfPenetration;
            FPVector3 newPosition2 = position2 + moveDirection * halfPenetration;

            // 返回调整后的碰撞点，避免过度偏移
            return (newPosition1 + newPosition2) * FP.Half;
        }
    }
}
