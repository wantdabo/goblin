using System.Collections.Generic;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine;

namespace Pipeline.Timeline.Layouts.Common
{
    /// <summary>
    /// 管线 Timeline 布局
    /// </summary>
    public class PipelineLayout : ScriptableObject
    {
        /// <summary>
        /// 模型
        /// </summary>
        public int model;
        /// <summary>
        /// 轨道布局映射 PipelineData.Instruct.Index
        /// </summary>
        public List<Dictionary<ushort, List<uint>>> tracks;
    }
}