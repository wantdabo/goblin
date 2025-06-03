using System;
using System.IO;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using MessagePack;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Layouts.Common;
using Pipeline.Timeline.Tracks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline
{
    /// <summary>
    /// 管线工作者
    /// </summary>
    public class PipelineWorker
    {
        /// <summary>
        /// 管线路径
        /// </summary>
        public string pipelinepath { get; private set; }
        /// <summary>
        /// 管线 ID
        /// </summary>
        public uint pipeline { get; private set;}
        /// <summary>
        /// 管线工作的 TimelineAsset
        /// </summary>
        public TimelineAsset timelineasset { get; private set; }
        /// <summary>
        /// 管线工作的 PipelineLayout
        /// </summary>
        public PipelineLayout layout { get; private set; }
        /// <summary>
        /// 管线模型
        /// </summary>
        public int model { get; set; }

        public PipelineWorker(string pipelinepath)
        {
            this.pipelinepath = pipelinepath;
            this.pipeline = uint.Parse(Path.GetFileNameWithoutExtension(pipelinepath));
            Load();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            PipelineWorkSpace.SavePipeline(pipeline, model, timelineasset);
        }

        /// <summary>
        /// 还原
        /// </summary>
        public void Revert()
        {
            Load();
        }
        
        /// <summary>
        /// 加载管线数据和布局
        /// </summary>
        public void Load()
        {
            var data = PipelineWorkSpace.ReadPipelineData(pipeline);
            timelineasset = ScriptableObject.CreateInstance<TimelineAsset>();
            layout = PipelineWorkSpace.ReadPipelineLayout(pipeline);
            model = layout.model;
            
            PipelineWorkSpace.SettingsTimeline(timelineasset, data, layout);
        }
    }
}