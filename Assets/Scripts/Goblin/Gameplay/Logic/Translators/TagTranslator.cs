using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
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
            ril.tags = ObjectCache.Get<Dictionary<ushort, int>>();
            foreach (var tag in info.tags) ril.tags.Add(tag.Key, tag.Value);
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, ril);
        }
    }
}