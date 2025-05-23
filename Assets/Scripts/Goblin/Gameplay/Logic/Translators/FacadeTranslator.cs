using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 外观翻译器
    /// </summary>
    public class FacadeTranslator : Translator<FacadeInfo, RIL_FACADE>
    {
        protected override ushort id => RIL_DEFINE.FACADE;
        
        protected override void OnRIL(FacadeInfo info, RIL_FACADE ril)
        {
            ril.model = info.model;
        }
    }
}