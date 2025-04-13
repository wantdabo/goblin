using Goblin.Gameplay.Common.SkillData.Common;
using Goblin.Gameplay.Logic.Common.GameplayDatas;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class KowtowExtension
    {
        /// <summary>
        /// Vector3Data 转 FPVector3
        /// </summary>
        /// <param name="vector">三维向量数据</param>
        /// <returns>FPVector3</returns>
        public static FPVector3 ToFPVector3(this Vector3Data vector)
        {
            return new FPVector3(vector.x * FP.EN3, vector.y* FP.EN3, vector.z* FP.EN3);
        }
        
        /// <summary>
        /// GPVector3 转 FPVector3
        /// </summary>
        /// <param name="vector">三维向量数据</param>
        /// <returns>FPVector3</returns>
        public static FPVector3 ToFPVector3(this GPVector3 vector)
        {
            return new FPVector3(vector.x * FP.EN3, vector.y* FP.EN3, vector.z* FP.EN3);
        }
    }
}