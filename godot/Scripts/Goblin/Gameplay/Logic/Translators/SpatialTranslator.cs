using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 空间信息翻译器
    /// </summary>
    public class SpatialTranslator : Translator<SpatialInfo, RIL_SPATIAL>
    {
        public override ushort id => RIL_DEFINE.SPATIAL;

        protected override int OnCalcHashCode(SpatialInfo info)
        {
            int hash = 17;
            hash = hash * 31 + info.actor.GetHashCode();
            hash = hash * 31 + info.position.x.GetHashCode();
            hash = hash * 31 + info.position.y.GetHashCode();
            hash = hash * 31 + info.position.z.GetHashCode();
            hash = hash * 31 + info.euler.x.GetHashCode();
            hash = hash * 31 + info.euler.y.GetHashCode();
            hash = hash * 31 + info.euler.z.GetHashCode();
            hash = hash * 31 + info.scale.GetHashCode();
            
            return hash;
        }

        protected override void OnRIL(SpatialInfo info, RIL_SPATIAL ril)
        {
            ril.position = info.position;
            ril.euler = info.euler;
            ril.scale = info.scale;
        }
    }
}