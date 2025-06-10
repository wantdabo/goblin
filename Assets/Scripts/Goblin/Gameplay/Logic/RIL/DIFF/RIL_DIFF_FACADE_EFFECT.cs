using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.DIFF
{
    /// <summary>
    /// 外观特效渲染指令差异
    /// </summary>
    public class RIL_DIFF_FACADE_EFFECT : IRIL_DIFF
    {
        public override ushort id => RIL_DEFINE.FACADE_EFFECT;

        /// <summary>
        /// 特效信息
        /// </summary>
        public EffectInfo effect { get; set; }
        
        protected override void OnReady()
        {
            effect = default;
        }

        protected override void OnReset()
        {
            effect = default;
        }

        protected override void OnClone(IRIL_DIFF clone)
        {
            if (clone is not RIL_DIFF_FACADE_EFFECT diff) return;

            diff.effect = effect;
        }
    }
}