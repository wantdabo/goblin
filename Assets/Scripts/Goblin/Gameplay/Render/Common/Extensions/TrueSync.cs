using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.Extensions
{
    public static class TrueSync
    {
        public static Vector2 ToVector2(this TSVector2 vector)
        {
            return new Vector2(vector.x.AsFloat(), vector.y.AsFloat());
        }
        
        public static Vector3 ToVector3(this TSVector vector)
        {
            return new Vector3(vector.x.AsFloat(), vector.y.AsFloat(), vector.z.AsFloat());
        }
        
        public static Vector4 ToVector4(this TSVector4 vector)
        {
            return new Vector4(vector.x.AsFloat(), vector.y.AsFloat(), vector.z.AsFloat(), vector.w.AsFloat());
        }
    }
}
