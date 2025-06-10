using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 外观特效翻译器
    /// </summary>
    public class FacadeEffectTranslator : Translator<FacadeInfo, RIL_FACADE_EFFECT>
    {
        public override ushort id => RIL_DEFINE.FACADE_EFFECT;
        
        protected override bool once => true;
        
        protected override int OnCalcHashCode(FacadeInfo info)
        {
            int hash = 17;

            foreach (var id in info.effects)
            {
                hash = hash * 31 + info.actor.GetHashCode();
                if (info.effectdict.TryGetValue(id, out var eff))
                {
                    hash = hash * 31 + eff.id.GetHashCode();
                    hash = hash * 31 + eff.elapsed.GetHashCode();
                    hash = hash * 31 + eff.effect.GetHashCode();
                    hash = hash * 31 + eff.type.GetHashCode();
                    hash = hash * 31 + eff.follow.GetHashCode();
                    hash = hash * 31 + eff.followmask.GetHashCode();
                    hash = hash * 31 + eff.duration.GetHashCode();
                    hash = hash * 31 + eff.position.GetHashCode();
                    hash = hash * 31 + eff.euler.GetHashCode();
                    hash = hash * 31 + eff.scale.GetHashCode();
                }
            }

            return hash;
        }

        protected override void OnRIL(FacadeInfo info, RIL_FACADE_EFFECT ril)
        {
            ril.effectdict = RILCache.Ensure<Dictionary<uint, EffectInfo>>();
            foreach (var effect in info.effectdict) ril.effectdict.Add(effect.Key, effect.Value);
        }
    }
}