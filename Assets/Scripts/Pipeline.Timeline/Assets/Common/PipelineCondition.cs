using System;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Sirenix.OdinInspector;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// 管线条件包装
    /// </summary>
    [Serializable]
    public class PipelineCondition
    {
        [ValueDropdown("@OdinDefineValueDropdown.GetConditionDefine()")]
        [LabelText("条件")]
        public ushort id = CONDITION_DEFINE.TEST;

        [ShowIf("@CONDITION_DEFINE.TEST == id")]
        [LabelText("测试条件")]
        public TestCondi testcondition;
        
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns>条件</returns>
        /// <exception cref="NotImplementedException">未找到条件</exception>
        public Condition GetCondition()
        {
            switch (id)
            {
                case CONDITION_DEFINE.TEST:
                    return testcondition;
                default:
                    throw new NotImplementedException($"condition with ID {id} is not implemented.");
            }
        }

        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <exception cref="NotImplementedException">未能正确设置条件</exception>
        public void SetCondition(Condition condition)
        {
            switch (condition.id)
            {
                case CONDITION_DEFINE.TEST:
                    id = CONDITION_DEFINE.TEST;
                        testcondition = condition as TestCondi;
                    break;
                default:
                    throw new NotImplementedException($"condition with ID {condition.id} is not implemented.");
            }
        }
    }
}