using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pipeline.Timeline.Common
{
    /// <summary>
    /// 管线火花指令包装
    /// </summary>
    [Serializable]
    public class PipelineSparkInstruct
    {
        [LabelText("火花触发范围")]
        [ValueDropdown("@OdinValueDropdown.GetSparkInfluenceDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "触发范围")] 
        public sbyte influence = SPARK_INSTR_DEFINE.FLOW;

        [LabelText("使用内置令牌")]
        public bool useinnertoken = true;

        [ShowIf("@true == useinnertoken")]
        [ValueDropdown("@OdinValueDropdown.GetSparkTokenDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "内置火花令牌")] 
        public string innertoken = SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN;

        [ShowIf("@false == useinnertoken")]
        public string customtoken;

        public string token => useinnertoken ? innertoken : customtoken;

        [LabelText("条件列表")]
        public List<PipelineCondition> conditions;

        [HideLabel]
        [SerializeReference, InlineProperty]
        [TypeFilter("GetFilteredTypes")]
        public InstructData instructData;

        private static IEnumerable<Type> GetFilteredTypes()
        {
            return typeof(InstructData).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(InstructData).IsAssignableFrom(t));
        }
    }
}