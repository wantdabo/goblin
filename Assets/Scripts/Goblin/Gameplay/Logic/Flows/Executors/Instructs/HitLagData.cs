using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 顿帧指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class HitLagData : InstructData
    {
        public override ushort id => INSTR_DEFINE.HIT_LAG;

        /// <summary>
        /// 顿帧类型
        /// </summary>
        [LabelText("顿帧类型")]
        [ValueDropdown("@OdinValueDropdown.GetHitLagTypeDefine()")]
        public byte type = HIT_LAG_DEFINE.TYPE_INSTANCE;
        
        /// <summary>
        /// 顿帧强度
        /// </summary>
        [LabelText("顿帧强度")]
        public uint strength;
        [ShowIf("@HIT_LAG_DEFINE.TYPE_ADDITIVE == type")]
        [LabelText("最大强度")]
        public uint strengthmax;

        /// <summary>
        /// 持续时间
        /// </summary>
        [LabelText("持续时间")]
        public uint duration;
        [ShowIf("@HIT_LAG_DEFINE.TYPE_ADDITIVE == type")]
        [LabelText("最大持续时间")]
        public uint durationmax;

        /// <summary>
        /// 叠加因子
        /// </summary>
        [ShowIf("@HIT_LAG_DEFINE.TYPE_ADDITIVE == type")]
        [LabelText("叠加因子")]
        public uint additivefactor;
    }
}