using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

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
        [LabelText("是否朝向攻击者")]
        public bool uselookatattacker = true;
        /// <summary>
        /// 是否受击运动
        /// </summary>
        [LabelText("是否受击运动")]
        public bool usehitmotion = false;
        /// <summary>
        /// 受击运动类型
        /// </summary>
        [ShowIf("@usehitmotion")]
        [LabelText("受击运动类型")]
        [ValueDropdown("@OdinValueDropdown.GetBehitMotionTypeDefine()")]
        public byte hitmotiontype = BEHIT_DEFINE.MOTION_SELF;
        /// <summary>
        /// 受击运动
        /// </summary>
        [ShowIf("@usehitmotion")]
        [LabelText("受击运动")]
        public IntVector3 hitmotion;
        
        public BeHitData()
        {
            et = FLOW_DEFINE.ET_FLOW_HIT;
        }
    }
}