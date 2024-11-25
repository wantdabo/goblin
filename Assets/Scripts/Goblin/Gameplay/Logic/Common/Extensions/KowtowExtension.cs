using Goblin.Gameplay.Common.SkillDatas.Common;
using Kowtow.Math;
using UnityEngine;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class KowtowExtension
    {
        /// <summary>
        /// Vector3Data 转 FPVector
        /// </summary>
        /// <param name="vectordata"></param>
        /// <returns></returns>
        public static FPVector3 ToFPVector3(this Vector3Data vectordata)
        {
            return new FPVector3(vectordata.x * FP.EN3, vectordata.y* FP.EN3, vectordata.z* FP.EN3);
        }
    }
}
