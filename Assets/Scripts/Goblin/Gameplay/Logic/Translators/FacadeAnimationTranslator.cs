using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 外观动画翻译器
    /// </summary>
    public class FacadeAnimationTranslator : Translator<FacadeInfo, RIL_FACADE_ANIMATION>
    {
        public override ushort id => RIL_DEFINE.FACADE_ANIMATION;

        protected override int OnCalcHashCode(FacadeInfo info)
        {
            int hash = 17;
            hash = hash * 31 + info.actor.GetHashCode();
            hash = hash * 31 + info.animstate.GetHashCode();
            hash = hash * 31 + (info.animname != null ? info.animname.GetHashCode() : 0);
            hash = hash * 31 + info.animelapsed.GetHashCode();
            hash = hash * 31 + info.effectincrement.GetHashCode();

            return hash;
        }

        protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
        {
            ril.animstate = info.animstate;
            ril.animname = info.animname;
            ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
        }
    }
}