using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class SphereDetectionActionData : DetectionActionData
    {
        public int radius { get; set; }
    }
}
