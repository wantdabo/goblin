using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 特效指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class EffectData : InstructData
    {
        public override ushort id => INSTR_DEFINE.EFFECT;

        /// <summary>
        /// 特效资源 ID
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.EffectValueDropdown()")]
        [LabelText("特效资源 ID")]
        public int effect;

        /// <summary>
        /// 特效类型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetEffectTypeDefine()")]
        [LabelText("特效类型")]
        public byte type;

        /// <summary>
        /// 特效持续时间类型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetEffectDurationTypeDefine()")]
        [LabelText("特效持续时间类型")]
        public byte durationtype = EFFECT_DEFINE.DURATION_TIMELINE;

        /// <summary>
        /// 特效持续时间(毫秒)
        /// </summary>
        [ShowIf("@EFFECT_DEFINE.DURATION_CUSTOM == durationtype")]
        [LabelText("特效持续时间(毫秒)")]
        public int duration = 1000;

        /// <summary>
        /// 特效跟随
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetEffectFollowDefine()")]
        [LabelText("特效跟随")]
        public byte follow;

        /// <summary>
        /// 挂点
        /// </summary>
        [ShowIf("@EFFECT_DEFINE.FOLLOW_MOUNT == follow")]
        [LabelText("特效挂点")]
        public ushort mount;

        /// <summary>
        /// 特效跟随掩码
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetEffectFollowMaskDefine()")]
        [LabelText("特效跟随掩码")]
        public int followmask;

        /// <summary>
        /// 特效位置
        /// </summary>
        [LabelText("特效位置")]
        public IntVector3 position;

        /// <summary>
        /// 特效旋转
        /// </summary>
        [LabelText("特效旋转")]
        public IntVector3 euler;

        /// <summary>
        /// 特效缩放
        /// </summary>
        [LabelText("特效缩放")]
        public int scale = 1000;
    }
}