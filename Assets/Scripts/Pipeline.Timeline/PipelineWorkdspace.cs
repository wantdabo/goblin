using System;
using System.Collections.Generic;
using System.IO;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline
{
    /// <summary>
    /// Pipeline 工作空间
    /// </summary>
    public class PipelineWorkspace
    {
        /// <summary>
        /// 管线存储目录
        /// </summary>
        public static string storedir { get; private set; } = $"{Application.dataPath}/GameRes/Raw/Pipelines/";
        
        /// <summary>
        /// 工作者
        /// </summary>
        public static PipelineWorker worker { get; private set; }

        /// <summary>
        /// 工作者初始化
        /// </summary>
        /// <param name="pipelinepath">管线路径</param>
        public static void Work(string pipelinepath)
        {
            var go = GameObject.Find("Pipeline");
            if (null == go) return;
            var panel = go.GetComponent<PipelinePanel>();
            if (null == panel) return;

            if (false == File.Exists(pipelinepath)) return;
            worker = new PipelineWorker(pipelinepath);
            panel.transform.GetComponent<PlayableDirector>().playableAsset = worker.asset;
            Selection.activeObject = panel.gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        /// <summary>
        /// 将管线数据转换为 TimelineAsset
        /// </summary>
        /// <param name="data">管线数据</param>
        /// <returns>TimelineAsset</returns>
        public static TimelineAsset PipelineDataToTimelineAsset(PipelineData data)
        {
            throw new Exception("Not implemented yet");
        }

        /// <summary>
        /// 将 TimelineAsset 转换为管线数据
        /// </summary>
        /// <param name="asset">TimelineAsset</param>
        /// <returns>管线数据</returns>
        public static PipelineData TimelineToPipelineData(TimelineAsset asset)
        {
            throw new Exception("Not implemented yet");
        }

        /// <summary>
        /// 获取所有管线文件
        /// </summary>
        /// <returns>所有管线文件</returns>
        public static List<(string name, string path)> GetPipelines()
        {
            var files = Directory.GetFiles(storedir, "*.pipeline");
            var result = new List<(string name, string path)>();
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (EditorConfig.location.PipelineInfos.TryGetValue(int.Parse(filename), out var pipelinecfg))
                {
                    filename += $" - {pipelinecfg.Name}";
                }

                result.Add((filename, file));
            }

            return result;
        }

        /// <summary>
        /// 验证管线 ID 的合法性
        /// </summary>
        /// <param name="createpipeline"></param>
        /// <returns>(YES/NO, Feedback)</returns>
        public static (bool ok, string errmsg) ValidPipeline(uint createpipeline)
        {
            if (0 == createpipeline)
            {
                return (false, "管线 ID 不可以为零");
            }

            var pipelinepath = $"{storedir}{createpipeline}.pipeline";
            if (File.Exists(pipelinepath))
            {
                return (false, "管线 ID 已经存在");
            }

            return (true, string.Empty);
        }
    }
}
