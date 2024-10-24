using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Logic.Common.Extensions
{
    public static class TrueSync
    {
        /// <summary>
        /// Vector3Data 转 TSVector
        /// </summary>
        /// <param name="vectordata"></param>
        /// <returns></returns>
        public static TSVector ToVector(this Vector3Data vectordata)
        {
            return new TSVector(vectordata.x * FP.EN3, vectordata.y* FP.EN3, vectordata.z* FP.EN3);
        }
    }
}
