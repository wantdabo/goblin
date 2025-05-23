using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// Movement 渲染指令翻译器
    /// </summary>
    public class MovementTranslator : Translator<MovementInfo, RIL_MOVEMENT>
    {
        protected override ushort id => RIL_DEFINE.MOVEMENT;
        
        protected override void OnRIL(MovementInfo info, RIL_MOVEMENT ril)
        {
            ril.motion = info.motion;
        }
    }
}