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
    public class Detection : Behavior<DetectionInfo>
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
        /// AABB 碰撞检测
        /// </summary>
        /// <param name="a">AABB-A</param>
        /// <param name="b">AABB-B</param>
        /// <returns>YES/NO</returns>
        public bool DetectAABB(AABB a, AABB b)
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
        /// 计算 AABB 包围盒
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>AABB 包围盒</returns>
        public AABB CalcAABB(ulong id)
        {
            if (false == stage.SeekBehaviorInfo(id, out ColliderInfo collider)) return default;

            return CalcAABB(collider);
        }

        /// <summary>
        /// 计算 AABB 包围盒
        /// </summary>
        /// <param name="collider">碰撞盒信息</param>
        /// <returns>AABB 包围盒</returns>
        public AABB CalcAABB(ColliderInfo collider)
        {
            if (false == stage.SeekBehaviorInfo(collider.id, out SpatialInfo spatial)) return default;

            FPVector3 position = spatial.position;
            FPQuaternion rotation = FPQuaternion.Euler(spatial.euler);
            switch (collider.shape)
            {
                case COLLIDER_DEFINE.BOX:
                    // 获取 BoxShape 的半尺寸
                    FPVector3 halfSize = collider.size * FP.Half;

                    // 定义 box 的 8 个顶点（以中心为基准）
                    var vertices = ObjectCache.Get<List<FPVector3>>();
                    vertices[0] = new FPVector3(-halfSize.x, -halfSize.y, -halfSize.z);
                    vertices[1] = new FPVector3(halfSize.x, -halfSize.y, -halfSize.z);
                    vertices[2] = new FPVector3(-halfSize.x, halfSize.y, -halfSize.z);
                    vertices[3] = new FPVector3(halfSize.x, halfSize.y, -halfSize.z);
                    vertices[4] = new FPVector3(-halfSize.x, -halfSize.y, halfSize.z);
                    vertices[5] = new FPVector3(halfSize.x, -halfSize.y, halfSize.z);
                    vertices[6] = new FPVector3(-halfSize.x, halfSize.y, halfSize.z);
                    vertices[7] = new FPVector3(halfSize.x, halfSize.y, halfSize.z);

                    // 初始化 AABB 的最小值和最大值
                    FPVector3 min = FPVector3.MaxValue;
                    FPVector3 max = FPVector3.MinValue;

                    // 对每个顶点应用旋转和位移，更新 min 和 max
                    foreach (var vertex in vertices)
                    {
                        FPVector3 transformedVertex = rotation * (vertex + collider.offset);
                        min = FPVector3.Min(min, transformedVertex);
                        max = FPVector3.Max(max, transformedVertex);
                    }
                    vertices.Clear();
                    ObjectCache.Set(vertices);

                    // 计算最终的 AABB
                    return new AABB
                    {
                        position = position + (min + max) * FP.Half,
                        size = max - min
                    };
                case COLLIDER_DEFINE.SPHERE:
                    // Sphere 不受旋转影响
                    return new AABB
                    {
                        position = position + rotation * collider.offset,
                        size = new FPVector3(collider.radius * 2)
                    };
            }

            return default;
        }
    }
}