using System;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Pipeline.Timeline.Assets.Common
{
    /// <summary>
    /// 管线条件包装
    /// </summary>
    [Serializable]
    public class PipelineCondition
    {
        [ValueDropdown("@OdinValueDropdown.GetConditionDefine()")]
        [HideLabel]
        public ushort id = CONDITION_DEFINE.INPUT;

        [ShowIf("@CONDITION_DEFINE.INPUT == id")]
        [HideLabel]
        public InputCondition inputcondition;
        
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns>条件</returns>
        /// <exception cref="NotImplementedException">未找到条件</exception>
        public Condition GetCondition()
        {
            switch (id)
            {
                case CONDITION_DEFINE.INPUT:
                    return inputcondition;
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
                case CONDITION_DEFINE.INPUT:
                    id = CONDITION_DEFINE.INPUT;
                        inputcondition = condition as InputCondition;
                    break;
                default:
                    throw new NotImplementedException($"condition with ID {condition.id} is not implemented.");
            }
        }
    }
}