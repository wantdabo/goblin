using Goblin.Gameplay.Common.Defines;
using System.Collections.Generic;

namespace Goblin.SkillPipelineEditor
{
    public class BuffIntPopupData : IntPopupData
    {
        public override Dictionary<int, string> data => new()
        {
            // { (int)BUFF_DEFINE.BUFF_10001, $"[BUFF_{BUFF_DEFINE.BUFF_10001}] 赋予感电" },
            // { (int)BUFF_DEFINE.BUFF_10002, $"[BUFF_{BUFF_DEFINE.BUFF_10002}] 引爆感电" },
        };
    }
}
