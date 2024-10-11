using System.Collections.Generic;
using UnityEngine;

namespace Goblin
{
    public class GizmosDrawer : MonoBehaviour
    {
        private static GizmosDrawer mins;

        public static GizmosDrawer I
        {
            get
            {
                if (null == mins)
                {
                    mins = new GameObject("GizmosDrawer").AddComponent<GizmosDrawer>();
                }

                return mins;
            }
        }

        public struct SphereDrawerInfo
        {
            public Vector3 center;
            public float radius;
            public Color color;
        }
        private List<SphereDrawerInfo> sphereDrawerInfos = new();
        public void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            sphereDrawerInfos.Add(new SphereDrawerInfo { center = center, radius = radius, color = color });
        }

        public struct CubeDrawerInfo
        {
            public Vector3 center;
            public Vector3 size;
            public Color color;
        }
        private List<CubeDrawerInfo> cubewireDrawerInfos = new();
        public void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            cubewireDrawerInfos.Add(new CubeDrawerInfo { center = center, size = size, color = color });
        }

        public struct CylinderDrawerInfo
        {
            public Vector3 center;
            public float radius;
            public float height;
            public Color color;
        }
        private List<CylinderDrawerInfo> cylinderDrawerInfos = new();
        public void DrawWireCylinder(Vector3 position, float radius, float height, Color color)
        {
            cylinderDrawerInfos.Add(new CylinderDrawerInfo { center = position, radius = radius, height = height, color = color });
        }

        private void OnDrawGizmos()
        {
            foreach (var info in sphereDrawerInfos)
            {
                Gizmos.color = info.color;
                GizmosDrawWireSphere(info.center, info.radius, 20);
            }
            sphereDrawerInfos.Clear();

            foreach (var info in cubewireDrawerInfos)
            {
                Gizmos.color = info.color;
                Gizmos.DrawWireCube(info.center, info.size);
            }
            cubewireDrawerInfos.Clear();

            foreach (var info in cylinderDrawerInfos)
            {
                Gizmos.color = info.color;
                GizmosDrawWireCylinder(info.center, info.radius, info.height);
            }
            cylinderDrawerInfos.Clear();
        }

        private void GizmosDrawWireSphere(Vector3 center, float radius, int latitudeSegments = 40)
        {
            int longitudeSegments = latitudeSegments;
            // ����γ�ߣ�ˮƽԲ����
            for (int i = 0; i <= latitudeSegments; i++)
            {
                float theta = Mathf.PI * i / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                Vector3 previousPoint = Vector3.zero;

                for (int j = 0; j <= longitudeSegments; j++)
                {
                    float phi = 2 * Mathf.PI * j / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    float x = radius * sinTheta * cosPhi;
                    float y = radius * cosTheta;
                    float z = radius * sinTheta * sinPhi;

                    Vector3 nextPoint = center + new Vector3(x, y, z);

                    if (j > 0)
                    {
                        Gizmos.DrawLine(previousPoint, nextPoint);
                    }
                    previousPoint = nextPoint;
                }
            }

            // ���ƾ��ߣ�����Բ����
            for (int i = 0; i <= longitudeSegments; i++)
            {
                float phi = 2 * Mathf.PI * i / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                Vector3 previousPoint = Vector3.zero;

                for (int j = 0; j <= latitudeSegments; j++)
                {
                    float theta = Mathf.PI * j / latitudeSegments;
                    float sinTheta = Mathf.Sin(theta);
                    float cosTheta = Mathf.Cos(theta);

                    float x = radius * sinTheta * cosPhi;
                    float y = radius * cosTheta;
                    float z = radius * sinTheta * sinPhi;

                    Vector3 nextPoint = center + new Vector3(x, y, z);

                    if (j > 0)
                    {
                        Gizmos.DrawLine(previousPoint, nextPoint);
                    }
                    previousPoint = nextPoint;
                }
            }
        }

        private void GizmosDrawWireCylinder(Vector3 position, float radius, float height)
        {
            Vector3 topCenter = position + (Vector3.up * height / 2);
            Vector3 bottomCenter = position - (Vector3.up * height / 2);

            DrawCircle(topCenter, radius);
            DrawCircle(bottomCenter, radius);
            DrawCylinderSides(topCenter, bottomCenter, radius);
        }

        private void DrawCircle(Vector3 center, float radius)
        {
            const int segments = 40;
            float angleStep = 360.0f / segments;

            Vector3 previousPoint = center + (Vector3.forward * radius);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector3 nextPoint = center + (Quaternion.Euler(0, angle, 0) * Vector3.forward * radius);
                Gizmos.DrawLine(previousPoint, center);
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }

        private void DrawCylinderSides(Vector3 topCenter, Vector3 bottomCenter, float radius)
        {
            const int segments = 40;
            float angleStep = 360.0f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep;
                Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
                Vector3 topPoint = topCenter + offset;
                Vector3 bottomPoint = bottomCenter + offset;
                Gizmos.DrawLine(topPoint, bottomPoint);
            }
        }
    }
}