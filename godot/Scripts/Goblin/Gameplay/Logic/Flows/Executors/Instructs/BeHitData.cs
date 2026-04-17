using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 受击指令数据
    /// </summary>
    [MessagePackObject(true)]
    public class BeHitData : InstructData
    {
        public override ushort id => INSTR_DEFINE.BEHIT;
        
        /// <summary>
        /// 是否朝向攻击者
        /// </summary>
        public bool uselookatattacker = true;
        /// <summary>
        /// 是否受击运动
        /// </summary>
        public bool usehitmotion = false;
        /// <summary>
        /// 受击运动类型
        /// </summary>
        public byte hitmotiontype = BEHIT_DEFINE.MOTION_SELF_FORWARD;
        /// <summary>
        /// 受击运动
        /// </summary>
        public IntVector3 hitmotion;
        
        public BeHitData()
        {
            et = FLOW_DEFINE.ET_FLOW_HIT;
        }
    }
}