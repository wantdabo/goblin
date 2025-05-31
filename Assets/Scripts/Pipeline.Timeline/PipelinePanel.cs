using System;
using System.IO;
using Goblin;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Misc;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

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
            /// <summary>
            /// 删除管线
            /// </summary>
            删除管线,
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
            var pipelines = PipelineWorkspace.GetPipelines();
            foreach (var pipeline in pipelines) result.Add(pipeline.name, pipeline.path);

            return result;
        }
        
        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [ValueDropdown("ModelValueDropdown", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择模型")]
        [OnValueChanged("OnModelChanged", true)]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("模型")]
        public int model;

        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]

        [BoxGroup("修改管线")]
        [PropertySpace(SpaceAfter = 5)]
        [GUIColor(0, 1,0 )]
        [Button("保存", ButtonSizes.Medium)]
        private void SavePipeline()
        {
        }
        
        [ShowIf("@tab == PanelTab.修改管线 && false == string.IsNullOrEmpty(pipelinepath)")]
        [BoxGroup("修改管线")]
        [PropertySpace(SpaceAfter = 5)]
        [GUIColor(1, 0,0 )]
        [Button("还原", ButtonSizes.Medium)]
        private void RevertPipeline()
        {
        }

        [ShowIf("@tab == PanelTab.创建管线")]
        [BoxGroup("创建管线")]
        [ValidateInput("ValidInputPipeline")]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("管线 ID")]
        public uint createpipeline;
        
        [ShowIf("@tab == PanelTab.创建管线")]
        [BoxGroup("创建管线")]
        [ValueDropdown("ModelValueDropdown", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "选择模型")]
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
            PipelineData data = new PipelineData();
            data.model = createmodel;
            
            var path = Path.Combine(PipelineWorkspace.storedir, $"{createpipeline}.pipeline");
            File.WriteAllBytes(path, MessagePackSerializer.Serialize(data));
            tab = PanelTab.修改管线;
        }
        
        /// <summary>
        /// 管线路径改变事件
        /// </summary>
        private void OnPipelinePathChanged()
        {
            PipelineWorkspace.Work(pipelinepath);
        }

        /// <summary>
        /// 模型改变事件
        /// </summary>
        private void OnModelChanged()
        {
            if (null == PipelineWorkspace.worker) return;
            PipelineWorkspace.worker.model = model;
        }
        
#if UNITY_EDITOR
        private void Awake()
        {
            var pipeliens = PipelineWorkspace.GetPipelines();
            if (null == pipeliens || 0 == pipeliens.Count) return;
            pipelinepath = pipeliens[0].path;
            PipelineWorkspace.Work(pipelinepath);
        }
#endif
 
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;
            if (null == PipelineWorkspace.worker) return;
            
            model = PipelineWorkspace.worker.model;
        }
#endif

        /// <summary>
        /// 获取模型下拉列表
        /// </summary>
        /// <returns>模型下拉列表</returns>
        private ValueDropdownList<int> ModelValueDropdown()
        {
            var result = new ValueDropdownList<int>();
            result.Add(new ValueDropdownItem<int>("None", 0));
            foreach (var modelcfg in EditorConfig.location.ModelInfos.DataList) result.Add(new ValueDropdownItem<int>(modelcfg.Res, modelcfg.Id));
            
            return result;
        }
        
        /// <summary>
        /// 验证创建管线 ID 的合法性
        /// </summary>
        /// <returns>YES/NO</returns>
        private bool ValidCreatePipeline()
        {
            return PipelineWorkspace.ValidPipeline(createpipeline).ok;
        }
        
        /// <summary>
        /// 验证创建管线 ID 的合法性
        /// </summary>
        /// <param name="createpipeline">管线 ID</param>
        /// <param name="errmsg">feedback 信息</param>
        /// <returns>YES/NO</returns>
        private bool ValidInputPipeline(uint createpipeline, ref string errmsg)
        {
            var result = PipelineWorkspace.ValidPipeline(createpipeline);
            errmsg = result.errmsg;
            
            return result.ok;
        }
    }
}