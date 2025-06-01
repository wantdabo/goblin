using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Goblin.Common;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;
using Goblin.Misc;
using MessagePack;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Assets.Common;
using Pipeline.Timeline.Layouts.Common;
using Pipeline.Timeline.Tracks.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Pipeline.Timeline
{
    /// <summary>
    /// Pipeline 工作空间
    /// </summary>
    public class PipelineWorkSpace
    {
        /// <summary>
        /// 管线数据存储目录
        /// </summary>
        public static string datadir { get; private set; } = $"{Application.dataPath}/GameRes/Raw/Pipelines/";
        /// <summary>
        /// 管线布局存储目录
        /// </summary>
        public static string layoutdir { get; private set; } = "Assets/Scripts/Pipeline.Timeline/Layouts/";
        /// <summary>
        /// PlayableDirector
        /// </summary>
        public static PlayableDirector director
        {
            get
            {
                var go = GameObject.Find("Pipeline");
                if (null == go) return default;
                var panel = go.GetComponent<PipelinePanel>();
                if (null == panel) return default;

                return panel.GetComponent<PlayableDirector>();
            }
        }
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
            if (false == File.Exists(pipelinepath)) return;
            if (null == director) return;
            worker = new PipelineWorker(pipelinepath);
        }

        /// <summary>
        /// 读取管线数据
        /// </summary>
        /// <param name="pipeline">管线 ID</param>
        /// <returns>管线数据</returns>
        public static PipelineData ReadPipelineData(uint pipeline)
        {
            var pipelinepath = Path.Combine(datadir, $"{pipeline}.pipeline");
            
            return ReadPipelineData(pipelinepath);
        }

        /// <summary>
        /// 读取管线数据
        /// </summary>
        /// <param name="pipelinepath">管线路径</param>
        /// <returns>管线数据</returns>
        public static PipelineData ReadPipelineData(string pipelinepath)
        {
            return MessagePackSerializer.Deserialize<PipelineData>(File.ReadAllBytes(pipelinepath));
        }

        public static PipelineLayout ReadPipelineLayout(uint pipeline)
        {
            var layoutpath = Path.Combine(layoutdir, $"{pipeline}.asset");
            if (false == File.Exists(layoutpath)) return default;

            return Object.Instantiate(AssetDatabase.LoadAssetAtPath<PipelineLayout>(layoutpath));
        }

        /// <summary>
        /// 创建管线数据
        /// </summary>
        /// <param name="pipeline">管线 ID</param>
        /// <param name="model">模型</param>
        public static void CreatePipeline(uint pipeline, int model)
        {
            PipelineData data = new PipelineData();
            var path = Path.Combine(datadir, $"{pipeline}.pipeline");
            File.WriteAllBytes(path, MessagePackSerializer.Serialize(data));
            
            var layout = ScriptableObject.CreateInstance<PipelineLayout>();
            layout.model = model;
            layout.name = $"{pipeline}";
            AssetDatabase.CreateAsset(layout, Path.Combine(layoutdir, layout.name + ".asset"));
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 保存管线数据
        /// </summary>
        /// <param name="pipeline">管线 ID</param>
        /// <param name="model">模型</param>
        /// <param name="asset">TimelineAsset</param>
        public static void SavePipeline(uint pipeline, int model, TimelineAsset asset)
        {
            if (null == asset) return;
            
            var layout = ScriptableObject.CreateInstance<PipelineLayout>();
            layout.name = $"{pipeline}";
            layout.model = model;
            layout.tracks = new();
            
            ScriptMachine.Begin();
            List<List<Instruct>> instructslist = new();
            var tracks = asset.GetOutputTracks();
            foreach (var track in tracks)
            {
                var clips = track.GetClips().ToList();
                if (0 == clips.Count) continue;

                var instructs = new List<Instruct>();
                instructslist.Add(instructs);
                
                foreach (var clip in clips)
                {
                    var pipelineasset = clip.asset as PipelineAsset;
                    if (null == pipelineasset) continue;
                    
                    var opt = ScriptMachine.Instruct((ulong)(clip.start * Config.Float2Int), (ulong)(clip.end * Config.Float2Int), pipelineasset.InstructData());
                    foreach (var condition in pipelineasset.conditions) opt.Condition(condition.GetCondition());

                    instructs.Add(opt.instruct);
                }
            }
            var data = ScriptMachine.End();
            
            if (data.Query(FLOW_DEFINE.OVERFLOW_LENGTH, out var instrinfos))
            {
                foreach (var instructs in instructslist)
                {
                    var tracklayout = new Dictionary<ushort, List<uint>>();
                    foreach (var instruct in instructs)
                    {
                        uint index = 0;
                        foreach (var instrinfo in instrinfos)
                        {
                            if (instrinfo.instruct != instruct) continue;
                            index = instrinfo.index;
                            break;
                        }
                        
                        if (0 == index) throw new System.Exception("Instruct index is zero, this should not happen");
                        
                        if (false == tracklayout.TryGetValue(instruct.data.id, out var indexes)) tracklayout.Add(instruct.data.id, indexes = new List<uint>());
                        indexes.Add(index);
                    }

                    layout.tracks.Add(tracklayout);
                }
            }

            var path = Path.Combine(datadir, $"{pipeline}.pipeline");
            File.WriteAllBytes(path, MessagePackSerializer.Serialize(data));
            
            AssetDatabase.DeleteAsset(Path.Combine(layoutdir, layout.name + ".asset"));
            AssetDatabase.CreateAsset(layout, Path.Combine(layoutdir, layout.name + ".asset"));
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取所有管线文件
        /// </summary>
        /// <returns>所有管线文件</returns>
        public static List<(string name, string path)> GetPipelines()
        {
            var files = Directory.GetFiles(datadir, "*.pipeline");
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

            var pipelinepath = $"{datadir}{createpipeline}.pipeline";
            if (File.Exists(pipelinepath))
            {
                return (false, "管线 ID 已经存在");
            }

            return (true, string.Empty);
        }
    }
}
