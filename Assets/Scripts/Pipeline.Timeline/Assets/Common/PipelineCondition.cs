using System;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Sirenix.OdinInspector;

namespace Pipeline.Timeline.Assets.Common
{
    [Serializable]
    public class PipelineCondition
    {
        [ValueDropdown("@OdinDefineValueDropdown.GetConditionDefine()")]
        [LabelText("条件")]
        public ushort id = CONDITION_DEFINE.TEST;

        [ShowIf("@CONDITION_DEFINE.TEST == id")]
        [LabelText("测试条件")]
        public TestCondi testcondition;
    }
}