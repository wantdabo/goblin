using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;

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
