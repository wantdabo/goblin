using System;
using System.IO;
using Goblin.Gameplay.Logic.Flows;
using MessagePack;
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
        /// 管线模型
        /// </summary>
        public int model { get; set; }
        /// <summary>
        /// 管线工作的 TimelineAsset
        /// </summary>
        public TimelineAsset asset { get; private set; }

        public PipelineWorker(string pipelinepath)
        {
            this.pipelinepath = pipelinepath;
            var data = MessagePackSerializer.Deserialize<PipelineData>(File.ReadAllBytes(this.pipelinepath));
            model = data.model;
            asset = ScriptableObject.CreateInstance<TimelineAsset>();
        }
    }
}