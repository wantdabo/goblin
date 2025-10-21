using System;
using System.Collections.Generic;
using System.IO;
using Goblin;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Misc;
using MessagePack;
using Pipeline.Timeline.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Object = System.Object;

namespace Pipeline.Timeline
{
    /// <summary>
    /// Pipeline 面板
    /// </summary>
    [ExecuteInEditMode]
    public class PipelinePanel : MonoBehaviour
    {
        /// <summary>
        /// 面板 Tab 枚举
        /// </summary>
        public enum PanelTab
        {
            /// <summary>
            /// 修改管线
            /// </summary>
            修改管线,
            /// <summary>
            /// 创建管线
            /// </summary>
            创建管线,
        }
        
        [EnumToggleButtons, HideLabel]
        public PanelTab tab = PanelTab.修改管线;

        [ShowIf("@tab == PanelTab.修改管线")]
        [BoxGroup("修改管线")]
        [ValueDropdown("PipelineValueDropdown", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择管线")]
        [OnValueChanged("OnPipelinePathChanged", true)]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("管线")]
        public string pipelinepath;
        private ValueDropdownList<string> PipelineValueDropdown()
        {
            var result = new ValueDropdownList<string>();
            var pipelines = PipelineWorkSpace.GetPipelines();
            foreach (var pipeline in pipelines) result.Add(pipeline.name, pipeline.path);

            return result;
        }
        
        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [ValueDropdown("@OdinValueDropdown.ModelValueDropdown()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择模型")]
        [OnValueChanged("OnModelChanged", true)]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("模型")]
        public int model;

        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [PropertySpace(SpaceAfter = -5)]
        [GUIColor(0, 1,0 )]
        [Button("保存", ButtonSizes.Medium)]
        private void SavePipeline()
        {
            if (null == PipelineWorkSpace.worker) return;
            PipelineWorkSpace.worker.Save();
        }
        
        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [PropertySpace(SpaceAfter = -5)]
        [GUIColor(0, 1,1 )]
        [Button("还原", ButtonSizes.Medium)]
        private void RevertPipeline()
        {
            if (null == PipelineWorkSpace.worker) return;
            PipelineWorkSpace.worker.Revert();
        }
        
        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [PropertySpace(SpaceAfter = 5)]
        [GUIColor(1, 0,0 )]
        [Button("删除", ButtonSizes.Medium)]
        private void DeletePipeline()
        {
        }

        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [LabelText("火花指令")]
        [PropertySpace(SpaceAfter = 20)]
        [ListDrawerSettings(OnBeginListElementGUI = nameof(BeginElement), OnEndListElementGUI = nameof(EndElement))]
        public PipelineSparkInstruct[] sparkinstructs;

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

        [ShowIf("@tab == PanelTab.创建管线")]
        [BoxGroup("创建管线")]
        [ValidateInput("ValidInputPipeline")]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("管线 ID")]
        public uint createpipeline;
        
        [ShowIf("@tab == PanelTab.创建管线")]
        [BoxGroup("创建管线")]
        [ValueDropdown("@OdinValueDropdown.ModelValueDropdown()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择模型")]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("模型")]
        public int createmodel;

        [ShowIf("@tab == PanelTab.创建管线")]
        [BoxGroup("创建管线")]
        [EnableIf("ValidCreatePipeline")]
        [PropertySpace(SpaceAfter = 5)]
        [GUIColor(0, 1,0 )]
        [Button("创建", ButtonSizes.Medium)]
        private void CreatePipeline()
        {
            PipelineWorkSpace.CreatePipeline(createpipeline, createmodel);
            tab = PanelTab.修改管线;
        }
        
        /// <summary>
        /// 管线路径改变事件
        /// </summary>
        private void OnPipelinePathChanged()
        {
            PipelineWorkSpace.Work(pipelinepath);
        }

        /// <summary>
        /// 模型改变事件
        /// </summary>
        private void OnModelChanged()
        {
            if (null == PipelineWorkSpace.worker) return;
            PipelineWorkSpace.worker.model = model;
        }
        
#if UNITY_EDITOR
        private void Awake()
        {
            var pipeliens = PipelineWorkSpace.GetPipelines();
            if (null == pipeliens || 0 == pipeliens.Count) return;
            pipelinepath = pipeliens[0].path;
            PipelineWorkSpace.Work(pipelinepath);
        }
#endif
 
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            if (null == PipelineWorkSpace.worker) return;
            pipelinepath = PipelineWorkSpace.worker.pipelinepath;
            model = PipelineWorkSpace.worker.model;
            
            PipelineWorkSpace.director.Evaluate();
        }
#endif
        
        /// <summary>
        /// 验证创建管线 ID 的合法性
        /// </summary>
        /// <returns>YES/NO</returns>
        private bool ValidCreatePipeline()
        {
            return PipelineWorkSpace.ValidPipeline(createpipeline).ok;
        }
        
        /// <summary>
        /// 验证创建管线 ID 的合法性
        /// </summary>
        /// <param name="createpipeline">管线 ID</param>
        /// <param name="errmsg">feedback 信息</param>
        /// <returns>YES/NO</returns>
        private bool ValidInputPipeline(uint createpipeline, ref string errmsg)
        {
            var result = PipelineWorkSpace.ValidPipeline(createpipeline);
            errmsg = result.errmsg;
            
            return result.ok;
        }
    }
}