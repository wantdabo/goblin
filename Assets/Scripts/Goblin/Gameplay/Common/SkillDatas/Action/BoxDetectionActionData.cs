using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class BoxDetectionActionData : DetectionActionData
    {
        public Vector3Data size { get; set; }
    }
}
