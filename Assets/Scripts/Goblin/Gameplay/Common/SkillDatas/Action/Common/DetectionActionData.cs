using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action.Common
{
    [MessagePackObject(true)]
    public abstract class DetectionActionData : SkillActionData
    {
        public Vector3Data position { get; set; }        
    }
}
