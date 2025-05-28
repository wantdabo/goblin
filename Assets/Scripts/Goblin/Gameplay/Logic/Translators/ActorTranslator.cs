using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// Actors 渲染指令翻译器
    /// </summary>
    public class ActorTranslator : Translator<StageInfo, RIL_ACTOR>
    {
        public override ushort id => RIL_DEFINE.ACTOR;

        protected override bool once => true;

        protected override int OnCalcHashCode(StageInfo info)
        {
            int hash = 17;
            foreach (var actor in info.actors)
            {
                hash = hash * 31 + actor.GetHashCode();
            }

            return hash;
        }

        protected override void OnRIL(StageInfo info, RIL_ACTOR ril)
        {
            foreach (var actor in info.actors) ril.actors.Add(actor);
        }
    }
}