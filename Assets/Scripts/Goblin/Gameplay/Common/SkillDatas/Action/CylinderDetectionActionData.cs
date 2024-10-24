using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class CylinderDetectionActionData : DetectionActionData
    {
        public int radius { get; set; }
        public int height { get; set; }
    }
}
