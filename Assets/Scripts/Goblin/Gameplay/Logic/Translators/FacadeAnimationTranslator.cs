using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 外观动画翻译器
    /// </summary>
    public class FacadeAnimationTranslator : Translator<FacadeInfo, RIL_FACADE_ANIMATION>
    {
        public override ushort id { get; }
        
        protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
        {
            ril.animstate = info.animstate;
            ril.animname = info.animname;
            ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
        }
    }
}