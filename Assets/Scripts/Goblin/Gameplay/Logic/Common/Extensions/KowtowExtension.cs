using Goblin.Gameplay.Logic.Common.GPDatas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class KowtowExtension
    {
        /// <summary>
        /// GPVector3 转 FPVector3
        /// </summary>
        /// <param name="vector">三维向量数据</param>
        /// <returns>FPVector3</returns>
        public static FPVector3 ToFPVector3(this GPVector3 vector)
        {
            return new FPVector3(vector.x * FP.EN3, vector.y* FP.EN3, vector.z* FP.EN3);
        }
        
        /// <summary>
        /// GPVector2 转 FPVector2
        /// </summary>
        /// <param name="vector">二维向量数据</param>
        /// <returns>FPVector2</returns>
        public static FPVector2 ToFPVector2(this GPVector2 vector)
        {
            return new FPVector2(vector.x * FP.EN3, vector.y* FP.EN3);
        }
    }
}