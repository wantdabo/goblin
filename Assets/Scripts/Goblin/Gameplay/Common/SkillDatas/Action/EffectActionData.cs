using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class EffectActionData : SkillActionData
    {
        public string res { get; set; }
        public Vector3Data position { get; set; }
        public Vector3Data eulerAngle { get; set; }
        public int scale { get; set; }
        public bool positionBinding { get; set; }
    }
}
