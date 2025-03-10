using Kowtow.Collision.Shapes;
using Kowtow.Math;

namespace Kowtow.Collision
{
    /// <summary>
    /// AABB 包围盒
    /// </summary>
    public struct AABB
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public FPVector3 center { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public FPVector3 size { get; set; }

        public override string ToString()
        {
            return $"AABB (center -> {center}, size -> {size})";
        }

        /// <summary>
        /// 从 Rigidbody 创建一个 AABB
        /// </summary>
        /// <param name="rigidbody">Rigidbody</param>
        /// <returns>AABB</returns>
        public static AABB CreateFromRigidbody(Rigidbody rigidbody)
        {
            return CreateFromShape(rigidbody.shape, rigidbody.position, rigidbody.rotation);
        }
        
        /// <summary>
        /// 从 Shape 创建一个 AABB
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>AABB</returns>
        public static AABB CreateFromShape(IShape shape, FPVector3 position, FPQuaternion rotation)
        {
            if (shape is BoxShape box)
            {
                // 获取 BoxShape 的半尺寸
                FPVector3 halfSize = box.size * FP.Half;

                // 定义 box 的 8 个顶点（以中心为基准）
                var vertices = ObjectPool.GetVertices(8);
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
                    FPVector3 transformedVertex = rotation * (vertex + box.center);
                    min = FPVector3.Min(min, transformedVertex);
                    max = FPVector3.Max(max, transformedVertex);
                }
                ObjectPool.SetVertices(vertices);

                // 计算最终的 AABB
                return new AABB
                {
                    center = position + (min + max) * FP.Half,
                    size = max - min
                };
            }
            else if (shape is SphereShape sphere)
            {
                // SphereShape 不受旋转影响
                return new AABB
                {
                    center = position + rotation * sphere.center,
                    size = new FPVector3(sphere.radius * 2)
                };
            }

            return default;
        }
    }
}
