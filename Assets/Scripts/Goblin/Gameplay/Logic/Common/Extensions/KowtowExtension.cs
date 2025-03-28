using Goblin.Gameplay.Common.SkillData.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class KowtowExtension
    {
        /// <summary>
        /// Vector3Data 转 FPVector3
        /// </summary>
        /// <param name="vectordata"></param>
        /// <returns>FPVector3</returns>
        public static FPVector3 ToFPVector3(this Vector3Data vectordata)
        {
            return new FPVector3(vectordata.x * FP.EN3, vectordata.y* FP.EN3, vectordata.z* FP.EN3);
        }
    }
}