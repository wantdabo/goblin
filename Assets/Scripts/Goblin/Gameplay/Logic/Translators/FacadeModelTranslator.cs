using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 外观翻译器
    /// </summary>
    public class FacadeModelTranslator : Translator<FacadeInfo, RIL_FACADE_MODEL>
    {
        public override ushort id => RIL_DEFINE.FACADE_MODEL;

        protected override int OnCalcHashCode(FacadeInfo info)
        {
            int hash = 17;
            hash = hash * 31 + info.actor.GetHashCode();
            hash = hash * 31 + info.model.GetHashCode();

            return hash;
        }

        protected override void OnRIL(FacadeInfo info, RIL_FACADE_MODEL ril)
        {
            ril.model = info.model;
        }
    }
}