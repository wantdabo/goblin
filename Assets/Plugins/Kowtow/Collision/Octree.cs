using Kowtow.Math;
using System;
using System.Collections.Generic;

namespace Kowtow.Collision
{
    /// <summary>
    /// 树节点
    /// </summary>
    public class OTNode
    {
        /// <summary>
        /// 节点 ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// AABB
        /// </summary>
        public AABB aabb { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public OTNode parent { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public OTNode[] childs { get; set; }
        /// <summary>
        /// Rigidbody 集合
        /// </summary>
        public List<Rigidbody> rigidbodies { get; set; }
    }

    /// <summary>
    /// 八叉树
    /// </summary>
    public class Octree
    {
        /// <summary>
        /// 最小尺寸
        /// </summary>
        public const int MIN_SIZE = 4;
        /// <summary>
        /// 最大尺寸
        /// </summary>
        public const int MAX_SIZE = 131072;
        /// <summary>
        /// 节点容量
        /// </summary>
        public const int NODE_CAPACITY = 4;
        /// <summary>
        /// 世界
        /// </summary>
        private World world { get; set; }
        /// <summary>
        /// 根节点
        /// </summary>
        public OTNode root { get; private set; }
        /// <summary>
        /// 节点集合
        /// </summary>
        private List<OTNode> nodes { get; set; } = new();
        /// <summary>
        /// Rigidbody 集合
        /// </summary>
        private List<Rigidbody> rigidbodies = new();
        /// <summary>
        /// Rigidbody 映射 OTNode
        /// </summary>
        private Dictionary<Rigidbody, OTNode> rigidbodymapping = new();

        /// <summary>
        /// 八叉树构造函数
        /// </summary>
        /// <param name="world">世界</param>
        public Octree(World world)
        {
            this.world = world;
            root = new OTNode();
            NodeSettings(null, -1, root);
        }

        /// <summary>
        /// 添加刚体
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        /// <exception cref="Exception">超出了空间范围</exception>
        public void Rigidbody2Node(Rigidbody rigidbody)
        {
            if (false == rigidbodies.Contains(rigidbody))
            {
                rigidbodies.Add(rigidbody);
            }

            if (false == Detection.InsideAABB(rigidbody.aabb, root.aabb))
            {
                throw new Exception($"out of range. otctree range [{MAX_SIZE}, {MAX_SIZE}, {MAX_SIZE}]");
            }
            Rigidbody2Node(rigidbody, root);
        }

        /// <summary>
        /// 移除刚体
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        public void RmvRigidbody(Rigidbody rigidbody)
        {
            if (rigidbodies.Contains(rigidbody))
            {
                rigidbodies.Remove(rigidbody);
            }

            var node = QueryNode(rigidbody);
            if (null == node) return;
            node.rigidbodies.Remove(rigidbody);
        }

        /// <summary>
        /// AABB 更新
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        public void AABBUpdate(Rigidbody rigidbody)
        {
            var node = QueryNode(rigidbody);
            if (null == node) return;

            rigidbodymapping.Remove(rigidbody);
            node.rigidbodies.Remove(rigidbody);
            Rigidbody2Node(rigidbody, node.parent);
        }

        /// <summary>
        /// 查询同空间的刚体集合
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        /// <param name="nearbybodies">相邻刚体列表</param>
        /// <returns>YES/NO</returns>
        public bool QueryRigidbodies(Rigidbody rigidbody, out List<Rigidbody> nearbybodies)
        {
            nearbybodies = default;
            var node = QueryNode(rigidbody);
            if (null == node) return false;

            nearbybodies = new();
            var p = node;
            while (null != p)
            {
                if (null != p.rigidbodies) nearbybodies.AddRange(p.rigidbodies);
                p = p.parent;
            }
            ChildRigidbodies(p, nearbybodies);

            return true;
        }

        /// <summary>
        /// 查询同空间的刚体集合
        /// </summary>
        /// <param name="aabb">AABB 包围盒</param>
        /// <param name="nearbybodies">相邻刚体列表</param>
        /// <returns>YES/NO</returns>
        public bool QueryRigidbodies(AABB aabb, out List<Rigidbody> nearbybodies)
        {
            nearbybodies = default;
            if (false == Detection.InsideAABB(aabb, root.aabb)) return false;

            nearbybodies = new();
            if (null != root.rigidbodies) nearbybodies.AddRange(root.rigidbodies);
            ChildRigidbodies(root, aabb, nearbybodies);

            return true;
        }

        /// <summary>
        /// 获取树子节点下的所有刚体
        /// </summary>
        /// <param name="node">树节点</param>
        /// <param name="nearbybodies">相邻刚体列表</param>
        private void ChildRigidbodies(OTNode node, List<Rigidbody> nearbybodies)
        {
            if (null == node) return;
            if (null == node.childs) return;

            for (int i = 0; i < node.childs.Length; i++)
            {
                var child = node.childs[i];

                if (null == child.rigidbodies) continue;

                nearbybodies.AddRange(child.rigidbodies);
                ChildRigidbodies(child, nearbybodies);
            }
        }

        /// <summary>
        /// 获取树子节点下的所有刚体
        /// </summary>
        /// <param name="node">树节点</param>
        /// <param name="aabb">AABB 包围盒</param>
        /// <param name="nearbybodies">相邻刚体列表</param>
        private void ChildRigidbodies(OTNode node, AABB aabb, List<Rigidbody> nearbybodies)
        {
            if (null == node) return;
            if (null == node.childs) return;

            for (int i = 0; i < node.childs.Length; i++)
            {
                var child = node.childs[i];
                if (null == child.rigidbodies) continue;
                if (false == Detection.InsideAABB(aabb, child.aabb)) continue;

                nearbybodies.AddRange(child.rigidbodies);
                ChildRigidbodies(child, aabb, nearbybodies);
            }
        }

        /// <summary>
        /// 通过刚体查询隶属树节点
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        /// <returns>树节点</returns>
        private OTNode QueryNode(Rigidbody rigidbody)
        {
            if (rigidbodymapping.TryGetValue(rigidbody, out var node)) return node;

            return default;
        }

        /// <summary>
        /// 刚体存入树节点
        /// </summary>
        /// <param name="rigidbody">刚体</param>
        /// <param name="node">树节点</param>
        private void Rigidbody2Node(Rigidbody rigidbody, OTNode node)
        {
            if (null == node)
            {
                Rigidbody2Node(rigidbody, root);

                return;
            }

            if (null == node.rigidbodies) node.rigidbodies = new List<Rigidbody>();

            // 动态规划
            ReSizeChildNodes(node);

            if (node.rigidbodies.Contains(rigidbody)) return;

            if (null != node.childs)
            {
                foreach (var child in node.childs)
                {
                    if (Detection.InsideAABB(rigidbody.aabb, child.aabb))
                    {
                        Rigidbody2Node(rigidbody, child);
                        return;
                    }
                }
            }

            if (Detection.InsideAABB(rigidbody.aabb, node.aabb))
            {
                rigidbodymapping.Add(rigidbody, node);
                node.rigidbodies.Add(rigidbody);

                return;
            }

            Rigidbody2Node(rigidbody, node.parent);
        }

        /// <summary>
        /// 重新规划树节点（扩容）
        /// </summary>
        /// <param name="node">树节点</param>
        private void ReSizeChildNodes(OTNode node)
        {
            if (MIN_SIZE >= node.size) return;
            if (null != node.rigidbodies && node.rigidbodies.Count <= NODE_CAPACITY) return;
            if (null != node.childs) return;

            node.childs = new OTNode[8];
            for (int i = 0; i < 8; i++)
            {
                node.childs[i] = new OTNode();
                NodeSettings(node, i, node.childs[i]);
            }
        }

        /// <summary>
        /// 树节点设置
        /// </summary>
        /// <param name="parent">树父节点</param>
        /// <param name="index">下标（切成 8 个方块，对应下标 0 - 7）</param>
        /// <param name="node">树节点</param>
        private void NodeSettings(OTNode parent, int index, OTNode node)
        {
            if (null == node) return;
            if (false == nodes.Contains(node)) nodes.Add(node);

            node.parent = parent;

            if (-1 == index)
            {
                node.id = "-1";
                node.size = MAX_SIZE;
                node.aabb = new AABB
                {
                    position = FPVector3.zero,
                    size = new FPVector3(MAX_SIZE, MAX_SIZE, MAX_SIZE),
                };
            }
            else
            {
                FP half = FP.Half;
                FP halfhalf = FP.Half * FP.Half;
                node.id = $"{parent.id} | {index}";
                node.size = parent.size / 2;
                FP offsetX = ((index & 1) == 0) ? -halfhalf : halfhalf;
                FP offsetY = ((index & 2) == 0) ? -halfhalf : halfhalf;
                FP offsetZ = ((index & 4) == 0) ? -halfhalf : halfhalf;
                node.aabb = new AABB
                {
                    position = parent.aabb.position + new FPVector3(offsetX, offsetY, offsetZ) * parent.size,
                    size = new FPVector3(half, half, half) * parent.size,
                };
            }
        }
    }
}
