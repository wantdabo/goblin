using Kowtow.Math;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.Extensions
{
    public static class FPExtension
    {
        /// <summary>
        /// FPVector2 转 Vector2
        /// </summary>
        /// <param name="fpvector2">FPVector2</param>
        /// <returns>Vector2</returns>
        public static Vector2 ToVector2(this FPVector2 fpvector2)
        {
            return new Vector2(fpvector2.x.AsFloat(), fpvector2.y.AsFloat());
        }
        
        /// <summary>
        /// FPVector3 转 Vector3
        /// </summary>
        /// <param name="fpvector3">FPVector3</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this FPVector3 fpvector3)
        {
            return new Vector3(fpvector3.x.AsFloat(), fpvector3.y.AsFloat(), fpvector3.z.AsFloat());
        }
        
        /// <summary>
        /// FPVector4 转 Vector4
        /// </summary>
        /// <param name="fpvector4">FPVector4</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this FPVector4 fpvector4)
        {
            return new Vector4(fpvector4.x.AsFloat(), fpvector4.y.AsFloat(), fpvector4.z.AsFloat(), fpvector4.w.AsFloat());
        }
        
        /// <summary>
        /// FPQuaternion 转 Quaternion
        /// </summary>
        /// <param name="fpquaternion">FPQuaternion</param>
        /// <returns>Quaternion</returns>
        public static Quaternion ToQuaternion(this FPQuaternion fpquaternion)
        {
            return new Quaternion(fpquaternion.x.AsFloat(), fpquaternion.y.AsFloat(), fpquaternion.z.AsFloat(), fpquaternion.w.AsFloat());
        }
    }
}
