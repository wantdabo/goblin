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
    [Serializable]
    public class PipelineSparkInstructBundle
    {
        [LabelText("条件列表")]
        public List<PipelineCondition> conditions;

        [LabelText("火花指令数据")]
        [SerializeReference, InlineProperty]
        [TypeFilter("@OdinValueDropdown.GetInstructDataFilteredTypes()")]
        public InstructData instructdata;
    }
    
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

        [LabelText("内置火花令牌")]
        [ShowIf("@true == useinnertoken")]
        [ValueDropdown("@OdinValueDropdown.GetSparkTokenDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "内置火花令牌")] 
        public string innertoken = SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN;

        [LabelText("自定义火花令牌")]
        [ShowIf("@false == useinnertoken")]
        public string customtoken;

        public string token => useinnertoken ? innertoken : customtoken;

        [LabelText("火花指令数据列表")]
        [PropertySpace(SpaceAfter = 20)]
        [ListDrawerSettings(OnBeginListElementGUI = nameof(BeginElement), OnEndListElementGUI = nameof(EndElement))]
        public List<PipelineSparkInstructBundle> instructbundles;

#if UNITY_EDITOR
        private void BeginElement(int index)
        {
            // 🎨 淡蓝交替背景
            var color = (index % 2 == 0)
                ? new Color(0.95f, 0.95f, 1f)
                : new Color(0.9f, 0.9f, 1f);
            UnityEngine.GUI.color = color;

            // 🏷️ 绘制序号框
            var rect = UnityEditor.EditorGUILayout.GetControlRect(false, 18);
            rect.x += 4;
            rect.y += 2;
            rect.width = 30;
            rect.height = 16;

            var style = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = UnityEngine.Color.magenta }
            };

            var labelColor = new Color(0.2f, 0.4f, 0.8f); // 深蓝底
            var oldBg = UnityEngine.GUI.backgroundColor;
            UnityEngine.GUI.backgroundColor = labelColor;
            UnityEngine.GUI.Box(rect, (index + 1).ToString(), style);
            UnityEngine.GUI.backgroundColor = oldBg;
        }

        private void EndElement(int index)
        {
            UnityEngine.GUI.color = UnityEngine.Color.white;
        }
#endif
    }
}