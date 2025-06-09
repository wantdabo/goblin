using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class FPExtension
    {
        /// <summary>
        /// IntVector3 转 FPVector3
        /// </summary>
        /// <param name="vector">三维向量数据</param>
        /// <returns>FPVector3</returns>
        public static FPVector3 ToFPVector3(this IntVector3 vector)
        {
            return new FPVector3(vector.x * FP.EN3, vector.y* FP.EN3, vector.z* FP.EN3);
        }
        
        /// <summary>
        /// IntVector2 转 FPVector2
        /// </summary>
        /// <param name="vector">二维向量数据</param>
        /// <returns>FPVector2</returns>
        public static FPVector2 ToFPVector2(this IntVector2 vector)
        {
            return new FPVector2(vector.x * FP.EN3, vector.y* FP.EN3);
        }
        
        /// <summary>
        /// 将 FPVector3 转换为 IntVector3
        /// </summary>
        /// <param name="vector">FPVector3</param>
        /// <returns>IntVector3</returns>
        public static IntVector3 ToIntVector3(this FPVector3 vector)
        {
            vector *= 1000;
            return new IntVector3((int)vector.x, (int)vector.y, (int)vector.z);
        }
        
        /// <summary>
        /// 将 FPVector2 转换为 IntVector2
        /// </summary>
        /// <param name="vector">FPVector2</param>
        /// <returns>IntVector2</returns>
        public static IntVector2 ToIntVector2(this FPVector2 vector)
        {
            vector *= 1000;
            return new IntVector2((int)vector.x, (int)vector.y);
        }
    }
}