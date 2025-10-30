using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Goblin.Common;
using Goblin.Gameplay.Logic.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;
using Goblin.Misc;
using MessagePack;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Assets.Common;
using Pipeline.Timeline.Common;
using Pipeline.Timeline.Layouts.Common;
using Pipeline.Timeline.Tracks;
using Pipeline.Timeline.Tracks.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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
        /// Pipeline 面板
        /// </summary>
        public static PipelinePanel panel{
            get
            {
                var go = GameObject.Find("Pipeline");
                if (null == go) return default;
                return go.GetComponent<PipelinePanel>();
            }
        }
        /// <summary>
        /// 工作者
        /// </summary>
        public static PipelineWorker worker { get; private set; }

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (false == EditorSceneManager.GetActiveScene().name.Equals("PipelineScene")) return;
            EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
            EditorSceneManager.OpenScene("Assets/GameRes/Scene/PipelineScene.unity");
        }

        /// <summary>
        /// 工作者初始化
        /// </summary>
        /// <param name="pipelinepath">管线路径</param>
        public static void Work(string pipelinepath)
        {
            if (false == File.Exists(pipelinepath)) return;
            if (null == director) return;
            
            if (null != worker) worker.Dispose();
            worker = new PipelineWorker(pipelinepath);
            worker.Load();
        }

        /// <summary>
        /// 根据 PipelineData & PipelineLayou 恢复 Timeline 布局
        /// </summary>
        /// <param name="data">PipelineData</param>
        public static void SettingsTimeline(PipelineData data)
        {
            var timelineasset = worker.timelineasset;
            var layout = worker.layout;
            director.playableAsset = timelineasset;
            if (null != timelineasset && null != layout)
            {
                foreach (var track in layout.tracks)
                {
                    foreach (var kv in track)
                    {
                        PipelineTrack pipetrack = default;
                        switch (kv.Key)
                        {
                            case INSTR_DEFINE.ANIMATION:
                                pipetrack = timelineasset.CreateTrack<PipelineAnimationTrack>();
                                break;
                            case INSTR_DEFINE.SPATIAL_POSITION:
                                pipetrack = timelineasset.CreateTrack<PipelineSpatialPositionTrack>();
                                break;
                            case INSTR_DEFINE.LAUNCH_SKILL:
                                pipetrack = timelineasset.CreateTrack<PipelineLaunchSkillTrack>();
                                break;
                            case INSTR_DEFINE.EFFECT:
                                pipetrack = timelineasset.CreateTrack<PipelineEffectTrack>();
                                break;
                            case INSTR_DEFINE.COLLISION:
                                pipetrack = timelineasset.CreateTrack<PipelineCollisionTrack>();
                                break;
                            case INSTR_DEFINE.RMV_ACTOR:
                                pipetrack = timelineasset.CreateTrack<PipelineRmvActorTrack>();
                                break;
                            case INSTR_DEFINE.CHANGE_STATE:
                                pipetrack = timelineasset.CreateTrack<PipelineChangeStateTrack>();
                                break;
                            case INSTR_DEFINE.SPARK:
                                pipetrack = timelineasset.CreateTrack<PipelineSparkTrack>();
                                break;
                        }

                        if (null == pipetrack) continue;

                        foreach (var index in kv.Value)
                        {
                            if (false == data.Query(index, out var instruct)) continue;
                            var clip = pipetrack.CreateDefaultClip();

                            clip.start = instruct.begin * Config.Int2Float;
                            clip.duration = instruct.end * Config.Int2Float - clip.start;

                            var pipeasset = clip.asset as PipelineAsset;
                            pipeasset.checkonce = instruct.checkonce;
                            foreach (var condition in instruct.conditions)
                            {
                                if (null == pipeasset.conditions) pipeasset.conditions = new();
                                var pipecondition = new PipelineCondition();
                                pipecondition.condition = condition;
                                pipeasset.conditions.Add(pipecondition);
                            }

                            pipeasset.instrdata = instruct.data;
                        }
                    }
                }
            }

            // 处理花火指令
            if (null != data.sparkinstructs)
            {
                panel.sparkinstructs = new PipelineSparkInstruct[data.sparkinstructs.Count];
                for (int i = 0; i < panel.sparkinstructs.Length; i++)
                {
                    var sparkinstruct = data.sparkinstructs[i];
                    
                    var useinnertoken = false;
                    foreach (var item in OdinValueDropdown.GetSparkTokenDefine())
                    {
                        if (item.Value != sparkinstruct.token) continue;
                        useinnertoken = true;
                        break;
                    }

                    var pipelinesparkinstruct = new PipelineSparkInstruct
                    {
                        influence = sparkinstruct.influence,
                        useinnertoken = useinnertoken,
                        innertoken = sparkinstruct.token,
                        customtoken = sparkinstruct.token,
                        conditions = new List<PipelineCondition>()
                    };
                    pipelinesparkinstruct.instructdata = sparkinstruct.data;
                    if (null != sparkinstruct.conditions)
                    {
                        foreach (var condition in sparkinstruct.conditions)
                        {
                            var pipecondition = new PipelineCondition();
                            pipecondition.condition = condition;
                            pipelinesparkinstruct.conditions.Add(pipecondition);
                        }
                    }
                    panel.sparkinstructs[i] = pipelinesparkinstruct;
                }
            }
        }

        /// <summary>
        /// 读取管线数据
        /// </summary>
        /// <param name="pipeline">管线 ID</param>
        /// <returns>管线数据</returns>
        public static PipelineData ReadPipelineData(uint pipeline)
        {
            var pipelinepath = Path.Combine(datadir, $"{pipeline}.bytes");
            
            return ReadPipelineData(pipelinepath);
        }

        /// <summary>
        /// 读取管线数据
        /// </summary>
        /// <param name="pipelinepath">管线路径</param>
        /// <returns>管线数据</returns>
        public static PipelineData ReadPipelineData(string pipelinepath)
        {
            var rawdata = MessagePackSerializer.Deserialize<PipelineRawData>(File.ReadAllBytes(pipelinepath));

            return rawdata.ToPipelineData();
        }

        public static PipelineLayout ReadPipelineLayout(uint pipeline)
        {
            var layoutpath = Path.Combine(layoutdir, $"{pipeline}.asset");
            if (false == File.Exists(layoutpath)) return default;
            var layout = AssetDatabase.LoadAssetAtPath<PipelineLayout>(layoutpath);
            layout.ReadTracks();

            return layout;
        }

        /// <summary>
        /// 创建管线数据
        /// </summary>
        /// <param name="pipeline">管线 ID</param>
        /// <param name="model">模型</param>
        public static void CreatePipeline(uint pipeline, int model)
        {
            PipelineData data = new PipelineData
            {
                length = 0,
                instructs = new List<Instruct>()
            };
            var path = Path.Combine(datadir, $"{pipeline}.bytes");
            File.WriteAllBytes(path, MessagePackSerializer.Serialize(data.ToPipelineRawData()));
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
        public static void SavePipeline(uint pipeline)
        {
            if (null == worker) return;
            
            ScriptMachine.Begin();
            List<List<Instruct>> instructslist = new();
            if (null != worker.timelineasset)
            {
                foreach (var track in worker.timelineasset.GetOutputTracks())
                {
                    var clips = track.GetClips().ToList();
                    if (0 == clips.Count) continue;

                    var instructs = new List<Instruct>();
                    instructslist.Add(instructs);

                    foreach (var clip in clips)
                    {
                        var pipelineasset = clip.asset as PipelineAsset;
                        if (null == pipelineasset) continue;

                        var opt = ScriptMachine.Instruct((ulong)(clip.start * Config.Float2Int), (ulong)(clip.end * Config.Float2Int), pipelineasset.instrdata, pipelineasset.checkonce);
                        if (null != pipelineasset.conditions) foreach (var pipelinecondition in pipelineasset.conditions) opt.Condition(pipelinecondition.condition);

                        instructs.Add(opt.instruct);
                    }
                }
            }

            // 处理火花指令
            if (null != panel.sparkinstructs)
            {
                foreach (var instruct in panel.sparkinstructs)
                {
                    var opt = ScriptMachine.Instruct(instruct.influence, instruct.token, instruct.instructdata);
                    foreach (var pipelinecondition in instruct.conditions) opt.Condition(pipelinecondition.condition);
                }
            }

            var data = ScriptMachine.End();
            File.WriteAllBytes(Path.Combine(datadir, $"{pipeline}.bytes"), MessagePackSerializer.Serialize(data.ToPipelineRawData()));

            var layout = ScriptableObject.CreateInstance<PipelineLayout>();
            layout.name = $"{pipeline}";
            layout.model = worker.model;
            layout.tracks = new();
            foreach (var instructs in instructslist)
            {
                var tracklayout = new Dictionary<ushort, List<uint>>();
                layout.tracks.Add(tracklayout);
                foreach (var instruct in instructs)
                {
                    uint index = 0;
                    foreach (var instr in data.instructs)
                    {
                        index++;
                        if (instr != instruct) continue;
                        break;
                    }

                    if (0 == index) throw new System.Exception("Instruct index is zero, this should not happen");

                    if (false == tracklayout.TryGetValue(instruct.data.id, out var indexes)) tracklayout.Add(instruct.data.id, indexes = new List<uint>());
                    indexes.Add(index);
                }
            }
            layout.WriteTracks();
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
            var files = Directory.GetFiles(datadir, "*.bytes");
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
        /// <param name="createpipeline">新管线 ID</param>
        /// <returns>(YES/NO, Feedback)</returns>
        public static (bool ok, string errmsg) ValidPipeline(uint createpipeline)
        {
            if (0 == createpipeline)
            {
                return (false, "管线 ID 不可以为零");
            }

            var pipelinepath = $"{datadir}{createpipeline}.bytes";
            if (File.Exists(pipelinepath))
            {
                return (false, "管线 ID 已经存在");
            }

            return (true, string.Empty);
        }
    }
}
