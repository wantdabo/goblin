using Goblin.Common;
using Godot;
using Kowtow.Math;

namespace Goblin.Gameplay.Render.Common.Extensions
{
    public static class VectorExtension
    {
        public static Vector2 ToVector2(this FPVector2 v) => new(v.x.AsFloat(), v.y.AsFloat());
        public static Vector3 ToVector3(this FPVector3 v) => new(v.x.AsFloat(), v.y.AsFloat(), v.z.AsFloat());
        public static Vector4 ToVector4(this FPVector4 v) => new(v.x.AsFloat(), v.y.AsFloat(), v.z.AsFloat(), v.w.AsFloat());
        public static Quaternion ToQuaternion(this FPQuaternion q) => new(q.x.AsFloat(), q.y.AsFloat(), q.z.AsFloat(), q.w.AsFloat());

        public static Vector3 ToVector3(this IntVector3 v) => new(v.x * Config.Int2Float, v.y * Config.Int2Float, v.z * Config.Int2Float);
        public static Vector2 ToVector2(this IntVector2 v) => new(v.x * Config.Int2Float, v.y * Config.Int2Float);
    }
}
