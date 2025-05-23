using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 子弹运动指令数据
    /// </summary>
    public class BulletMotionData : InstructData
    {
        public override ushort id => INSTR_DEFINE.BULLET_MOTION;
        
        /// <summary>
        /// 子弹运动类型
        /// </summary>
        public ushort motion { get; set; }
    }
}