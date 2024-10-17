using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class SpatialActionData : SkillActionData
    {
        public Vector3Data position { get; set; }
        public int scale { get; set; }
    }
}
