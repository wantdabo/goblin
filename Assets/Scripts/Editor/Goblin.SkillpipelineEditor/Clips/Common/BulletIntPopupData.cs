using Goblin.Gameplay.Common.Defines;
using System.Collections.Generic;

namespace Goblin.SkillPipelineEditor
{
    public class BulletIntPopupData : IntPopupData
    {
        public override Dictionary<int, string> data => new()
        {
            { (int)BULLET_DEFINE.BULLET_10001, $"[BULLET_{BULLET_DEFINE.BULLET_10001}] 雷球" },
        };
    }
}
