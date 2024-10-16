using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    public class BreakTokenType
    {
        public const int NONE = 0;
        public const int JOYSTICK = 1;
        public const int RECV_HURT = 2;
        public const int RECV_CONTROL = 4;
        public const int SKILL_CAST = 8;
        public const int COMBO_SKILL_CAST = 16;
    }

    [MessagePackObject(true)]
    public class SkillBreakEventActionData : SkillActionData
    {
        public int token { get; set; }
    }
}
