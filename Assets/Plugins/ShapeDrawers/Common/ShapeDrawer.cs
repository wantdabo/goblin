using System.Collections.Generic;
using UnityEngine;

namespace ShapeDrawers.Common
{
    public class ShapeDrawer
    {
        private static Dictionary<DrawerType, Queue<Drawer>> pools = new();
        private static List<Drawer> drawings = new();

        public static void Clear()
        {
            foreach (var drawer in drawings)
            {
                drawer.Disable();
                SetDrawer(drawer);
            }
        }

        private static bool GetDrawer(DrawerType type, out Drawer drawer)
        {
            drawer = default;
            if (false == pools.TryGetValue(type, out var queue) || 0 == queue.Count) return false;
            while (true)
            {
                if (0 == queue.Count) break;
                drawer = queue.Dequeue();
                if (null != drawer.gameObject) break;
            }

            return null != drawer.gameObject;
        }

        private static bool GetDrawer<T>(DrawerType type, out T drawer) where T : Drawer
        {
            drawer = default;
            if (false == GetDrawer(type, out var baseDrawer)) return false;
            drawer = baseDrawer as T;

            return true;
        }

        private static void SetDrawer(Drawer drawer)
        {
            if (false == pools.TryGetValue(drawer.type, out var queue))
            {
                queue = new();
                pools.Add(drawer.type, queue);
            }
            if (queue.Contains(drawer)) return;
            queue.Enqueue(drawer);
        }

        public static void DrawBox(Vector3 position, Vector3 size, Quaternion rotation, Color color)
        {
            if (false == GetDrawer<BoxDrawer>(DrawerType.Box, out var drawer)) drawer = new BoxDrawer();
            drawer.Enabled();
            drawer.Settings(position, rotation, size);
            drawer.SetColor(color);
            drawings.Add(drawer);
        }

        public static void DrawSphere(Vector3 position, float radius, Color color)
        {
            if (false == GetDrawer<SphereDrawer>(DrawerType.Sphere, out var drawer)) drawer = new SphereDrawer();
            drawer.Enabled();
            drawer.Settings(position, radius);
            drawer.SetColor(color);
            drawings.Add(drawer);
        }

        public static void DrawCylinder(Vector3 position, float radius, float height, Quaternion rotation, Color color)
        {
            if (false == GetDrawer<CylinderDrawer>(DrawerType.Cylinder, out var drawer)) drawer = new CylinderDrawer();
            drawer.Enabled();
            drawer.Settings(position, radius, height, rotation);
            drawer.SetColor(color);
            drawings.Add(drawer);
        }
    }
}
