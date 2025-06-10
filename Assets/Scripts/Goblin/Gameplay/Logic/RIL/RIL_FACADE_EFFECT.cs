using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 外观特效指令
    /// </summary>
    public class RIL_FACADE_EFFECT : IRIL
    {
        public override ushort id => RIL_DEFINE.FACADE_EFFECT;
        
        /// <summary>
        /// 特效信息集合
        /// </summary>
        public Dictionary<uint, EffectInfo> effectdict { get; set; }
        
        protected override void OnReady()
        {
            effectdict = RILCache.Ensure<Dictionary<uint, EffectInfo>>();
        }

        protected override void OnReset()
        {
            effectdict.Clear();
            RILCache.Set(effectdict);
        }
    }
}