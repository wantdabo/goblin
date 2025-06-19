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
            private static Mesh raymesh;
            private static Mesh cubemesh;
            private static Mesh spheremesh;
            private Material material;

            private static Queue<(Vector3 center, Vector3 dire, float dis, Color color)> rayqueue = new();
            private static Queue<(Vector3 center, Quaternion rotation, Vector3 size, Color color)> cubequeue = new();
            private static Queue<(Vector3 center, float radius, Color color)> spherequeue = new();

            public static void DrawRay(Vector3 center, Vector3 dire, float dis, Color color)
            {
                rayqueue.Enqueue((center, dire, dis, color));
            }

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

                if (raymesh == null) raymesh = CreateRay();
                if (cubemesh == null) cubemesh = CreateWireframeCube();
                if (spheremesh == null) spheremesh = CreateWireframeSphere();
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("DrawPhys");
                Shader shader = Shader.Find("Unlit/Color");
                material ??= new Material(shader);

                var mpb = new MaterialPropertyBlock();

                while (rayqueue.TryDequeue(out var ray))
                {
                    mpb.SetColor("_Color", ray.color);
                    var matrix = Matrix4x4.TRS(ray.center, Quaternion.LookRotation(ray.dire), new Vector3(1f, 1f, ray.dis));
                    cmd.DrawMesh(raymesh, matrix, material, 0, -1, mpb);
                }

                while (cubequeue.TryDequeue(out var cube))
                {
                    mpb.SetColor("_Color", cube.color);
                    var matrix = Matrix4x4.TRS(cube.center, cube.rotation, cube.size);
                    cmd.DrawMesh(cubemesh, matrix, material, 0, -1, mpb);
                }

                while (spherequeue.TryDequeue(out var sphere))
                {
                    mpb.SetColor("_Color", sphere.color);
                    var matrix = Matrix4x4.TRS(sphere.center, Quaternion.identity, Vector3.one * sphere.radius);
                    cmd.DrawMesh(spheremesh, matrix, material, 0, -1, mpb);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            private static Mesh CreateRay()
            {
                var mesh = new Mesh();
                mesh.vertices = new[]
                {
                    Vector3.zero,
                    Vector3.forward
                };
                mesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0);
                mesh.RecalculateBounds();
                return mesh;
            }

            private static Mesh CreateWireframeCube()
            {
                var mesh = new Mesh();
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
                int[] indices = {
                    0,1, 1,2, 2,3, 3,0,
                    4,5, 5,6, 6,7, 7,4,
                    0,4, 1,5, 2,6, 3,7
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
                    float a1 = Mathf.PI * lat / latitudeSegments;
                    float y = Mathf.Cos(a1);
                    float r = Mathf.Sin(a1);

                    for (int lon = 0; lon <= longitudeSegments; lon++)
                    {
                        float a2 = 2 * Mathf.PI * lon / longitudeSegments;
                        float x = r * Mathf.Cos(a2);
                        float z = r * Mathf.Sin(a2);
                        vertices.Add(new Vector3(x, y, z) * 0.5f);
                    }
                }

                for (int lat = 0; lat <= latitudeSegments; lat++)
                {
                    for (int lon = 0; lon < longitudeSegments; lon++)
                    {
                        int current = lat * (longitudeSegments + 1) + lon;
                        indices.Add(current);
                        indices.Add(current + 1);
                    }
                }

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
