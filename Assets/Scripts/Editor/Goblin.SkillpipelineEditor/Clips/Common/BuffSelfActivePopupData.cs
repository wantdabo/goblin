using Goblin.Gameplay.Common.Defines;
using System.Collections.Generic;

namespace Goblin.SkillPipelineEditor
{
    public class BuffSelfActivePopupData : IntPopupData
    {
        public override Dictionary<int, string> data => new()
        {
            { BUFF_DEFINE.ACTIVE_SELF_NONE, "自身不生效" },
            { BUFF_DEFINE.ACTIVE_SELF_TIMELINE, "自身时间轴生效" },
            { BUFF_DEFINE.ACTIVE_SELF_HIT, "自身击中后生效" },
        };
    }
}
