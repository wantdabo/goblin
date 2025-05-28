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
    /// 标签翻译器
    /// </summary>
    public class TagTranslator : Translator<TagInfo, RIL_TAG>
    {
        public override ushort id => RIL_DEFINE.TAG;

        protected override void OnRIL(TagInfo info, RIL_TAG ril)
        {
            foreach (var tag in info.tags) ril.tags.Add(tag.Key, tag.Value);
        }
    }
}