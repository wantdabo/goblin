using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 标签翻译器
    /// </summary>
    public class TagTranslator : Translator<TagInfo>
    {
        protected override void OnRIL(TagInfo info)
        {
            RIL_TAG ril = new RIL_TAG();
            ril.actortype = info.tags.GetValueOrDefault(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.NONE);
            ril.hashero = info.tags.ContainsKey(TAG_DEFINE.HERO_ID);
            ril.hero = info.tags.GetValueOrDefault(TAG_DEFINE.HERO_ID, 0);
            ril.model = info.tags.GetValueOrDefault(TAG_DEFINE.MODEL_ID, 0);
            
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, ril);
        }
    }
}