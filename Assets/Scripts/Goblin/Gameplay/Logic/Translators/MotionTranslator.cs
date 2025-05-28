using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 运动信息翻译器
    /// </summary>
    public class MotionTranslator : Translator<MotionInfo, RIL_MOTION>
    {
        public override ushort id => RIL_DEFINE.MOTION;
        
        protected override void OnRIL(MotionInfo info, RIL_MOTION ril)
        {
            ril.motion = info.motion;
        }
    }
}