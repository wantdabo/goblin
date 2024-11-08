using Goblin.Gameplay.Common.Defines;
using System.Collections.Generic;

namespace Goblin.SkillPipelineEditor
{
    public class BuffTargetActivePopupData : IntPopupData
    {
        public override Dictionary<int, string> data => new()
        {
            { BUFF_DEFINE.ACTIVE_TARGET_NONE, "目标不生效" },
            { BUFF_DEFINE.ACTIVE_TARGET_HIT, "目标击中后生效" },
        };
    }
}
