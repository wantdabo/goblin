using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.Extensions
{
    public static class TrueSync
    {
        /// <summary>
        /// TSVector2 转 Vector2
        /// </summary>
        /// <param name="tsvector2">TSVector2</param>
        /// <returns>Vector2</returns>
        public static Vector2 ToVector2(this TSVector2 tsvector2)
        {
            return new Vector2(tsvector2.x.AsFloat(), tsvector2.y.AsFloat());
        }
        
        /// <summary>
        /// TSVector 转 Vector3
        /// </summary>
        /// <param name="tsvector">TSVector</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this TSVector tsvector)
        {
            return new Vector3(tsvector.x.AsFloat(), tsvector.y.AsFloat(), tsvector.z.AsFloat());
        }
        
        /// <summary>
        /// TSVector4 转 Vector4
        /// </summary>
        /// <param name="vector4">TSVector4</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this TSVector4 vector4)
        {
            return new Vector4(vector4.x.AsFloat(), vector4.y.AsFloat(), vector4.z.AsFloat(), vector4.w.AsFloat());
        }
        
        /// <summary>
        /// TSQuaternion 转 Quaternion
        /// </summary>
        /// <param name="tsquaternion">TSQuaternion</param>
        /// <returns>Quaternion</returns>
        public static Quaternion ToQuaternion(this TSQuaternion tsquaternion)
        {
            return new Quaternion(tsquaternion.x.AsFloat(), tsquaternion.y.AsFloat(), tsquaternion.z.AsFloat(), tsquaternion.w.AsFloat());
        }
    }
}
