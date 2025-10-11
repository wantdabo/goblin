using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 动画指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class AnimationData : InstructData
    {
        public override ushort id => INSTR_DEFINE.ANIMATION;

        /// <summary>
        /// 动画名称
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetModelAnimNames(PipelineWorkSpace.worker.model)")]
        [LabelText("动画名称")]
        public string name;
    }
}