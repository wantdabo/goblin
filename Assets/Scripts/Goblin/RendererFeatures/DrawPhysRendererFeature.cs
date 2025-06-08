using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Goblin.RendererFeatures
{
    public class DrawPhysRendererFeature : ScriptableRendererFeature
    {
        private DrawPhysPass pass;

        public override void Create()
        {
            pass = new DrawPhysPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        public class DrawPhysPass : ScriptableRenderPass
        {
            private static Mesh cubemesh;
            private static Mesh spheremesh;
            private Material material;

            private static Queue<(Vector3 center, Quaternion rotation, Vector3 size, Color color)> cubequeue = new();
            private static Queue<(Vector3 center, float radius, Color color)> spherequeue = new();
            
            public static void DrawCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
            {
                cubequeue.Enqueue((center, rotation, size, color));
            }
        
            public static void DrawSphere(Vector3 center, float radius, Color color)
            {
                spherequeue.Enqueue((center, radius, color));
            }

            public DrawPhysPass()
            {
                renderPassEvent = RenderPassEvent.AfterRendering;

                if (null == cubemesh) cubemesh = CreateWireframeCube();
                if (null == spheremesh) spheremesh = CreateWireframeSphere();

                Shader shader = Shader.Find("Unlit/Color");
                material = new Material(shader);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("DrawPhys");

                while (cubequeue.TryDequeue(out var cube))
                {
                    material.SetColor("_Color", cube.color);
                    var matrix = Matrix4x4.TRS(cube.center, cube.rotation, cube.size);
                    cmd.DrawMesh(cubemesh, matrix, material);
                }

                while (spherequeue.TryDequeue(out var sphere))
                {
                    material.SetColor("_Color", sphere.color);
                    var matrix = Matrix4x4.TRS(sphere.center, Quaternion.identity, new Vector3(sphere.radius, sphere.radius, sphere.radius));
                    cmd.DrawMesh(cubemesh, matrix, material); 
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            // 生成立方体线框 Mesh (用线段连接顶点)
            private static Mesh CreateWireframeCube()
            {
                var mesh = new Mesh();

                // 8 个顶点
                Vector3[] vertices = {
                    new(-0.5f, -0.5f, -0.5f),
                    new(0.5f, -0.5f, -0.5f),
                    new(0.5f, -0.5f, 0.5f),
                    new(-0.5f, -0.5f, 0.5f),
                    new(-0.5f, 0.5f, -0.5f),
                    new(0.5f, 0.5f, -0.5f),
                    new(0.5f, 0.5f, 0.5f),
                    new(-0.5f, 0.5f, 0.5f),
                };

                // 12 条线段，每条线段两个顶点索引
                int[] indices = {
                    0,1, 1,2, 2,3, 3,0, // 底面
                    4,5, 5,6, 6,7, 7,4, // 顶面
                    0,4, 1,5, 2,6, 3,7  // 竖直边
                };

                mesh.vertices = vertices;
                mesh.SetIndices(indices, MeshTopology.Lines, 0);
                mesh.RecalculateBounds();

                return mesh;
            }

            private static Mesh CreateWireframeSphere(int latitudeSegments = 12, int longitudeSegments = 24)
            {
                var mesh = new Mesh();

                var vertices = new List<Vector3>();
                var indices = new List<int>();

                for (int lat = 0; lat <= latitudeSegments; lat++)
                {
                    float a1 = Mathf.PI * lat / latitudeSegments; // 纬度角0 ~ pi
                    float y = Mathf.Cos(a1); // Y坐标
                    float r = Mathf.Sin(a1); // 半径（水平投影）

                    for (int lon = 0; lon <= longitudeSegments; lon++)
                    {
                        float a2 = 2 * Mathf.PI * lon / longitudeSegments; // 经度角0 ~ 2pi
                        float x = r * Mathf.Cos(a2);
                        float z = r * Mathf.Sin(a2);
                        vertices.Add(new Vector3(x, y, z) * 0.5f); // 半径0.5单位球
                    }
                }

                // 连接纬线（横向线条）
                for (int lat = 0; lat <= latitudeSegments; lat++)
                {
                    for (int lon = 0; lon < longitudeSegments; lon++)
                    {
                        int current = lat * (longitudeSegments + 1) + lon;
                        indices.Add(current);
                        indices.Add(current + 1);
                    }
                }

                // 连接经线（纵向线条）
                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    for (int lat = 0; lat < latitudeSegments; lat++)
                    {
                        int current = lat * (longitudeSegments + 1) + lon;
                        indices.Add(current);
                        indices.Add(current + longitudeSegments + 1);
                    }
                }

                mesh.vertices = vertices.ToArray();
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
                mesh.RecalculateBounds();

                return mesh;
            }

        }
    }
}
