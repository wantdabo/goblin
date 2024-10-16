using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.SkillPipelineEditor;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

namespace Goblin.Custom
{
    public class AssetsEditor : Editor
    {
        [MenuItem("Assets/美术/压缩动画")]
        public static void CompressAnimationClip()
        {
            var objs = Selection.objects;
            if (null == objs) return;

            foreach (var obj in objs)
            {
                if (obj is AnimationClip)
                {
                    var clip = obj as AnimationClip;
                    AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip);
                    Keyframe key;
                    Keyframe[] keyFrames;
                    for (int ii = 0; ii < curves.Length; ++ii)
                    {
                        AnimationClipCurveData curveDate = curves[ii];
                        if (curveDate.curve == null || curveDate.curve.keys == null)
                        {
                            continue;
                        }
                        keyFrames = curveDate.curve.keys;
                        for (int i = 0; i < keyFrames.Length; i++)
                        {
                            key = keyFrames[i];
                            key.value = float.Parse(key.value.ToString("f3"));
                            key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                            key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                            keyFrames[i] = key;
                        }
                        curveDate.curve.keys = keyFrames;
                        clip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                    }

                    // Save the modified clip
                    string path = AssetDatabase.GetAssetPath(clip);
                    AssetDatabase.CreateAsset(Object.Instantiate(clip), path);
                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Assets/技能管线读取数据")]
        public static void SkillPipelineDataRead()
        {
            var objs = Selection.objects;
            if (null == objs || 0 == objs.Length) return;
            var obj = objs[0];

            var asset = obj as Asset;
            uint length = Convert.ToUInt32(asset.Length * GameDef.SP_DATA_FRAME);
            List<ActionClip> clips = new();
            foreach (var group in asset.groups)
            {
                foreach (var track in group.Tracks)
                {
                    clips.AddRange(track.Clips);
                }
            }

            List<(ushort, byte[])> actionDatas = new();
            foreach (var clip in clips)
            {
                (ushort, byte[]) actionData = default;
                if (clip is EditorAnimationClip animationClip)
                {
                    var val = new AnimationActionData();
                    val.id = SkillActionDef.ANIMATION;
                    val.sframe = Convert.ToUInt32(clip.StartTime * GameDef.SP_DATA_FRAME);
                    val.eframe = Convert.ToUInt32(clip.EndTime * GameDef.SP_DATA_FRAME);
                    val.name = animationClip.animationClip.name;
                    actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
                }
                else if (clip is EditorSpatialClip spatialClip)
                {
                    var val = new SpatialActionData();
                    val.id = SkillActionDef.SPATIAL;
                    val.sframe = Convert.ToUInt32(clip.StartTime * GameDef.SP_DATA_FRAME);
                    val.eframe = Convert.ToUInt32(clip.EndTime * GameDef.SP_DATA_FRAME);
                    val.position = new Vector3Data(
                        Convert.ToInt32(spatialClip.position.x * Config.float2Int),
                        Convert.ToInt32(spatialClip.position.y * Config.float2Int),
                        Convert.ToInt32(spatialClip.position.z * Config.float2Int)
                    );
                    val.eulerAngle = new Vector3Data(
                        Convert.ToInt32(spatialClip.eulerAngle.x * Config.float2Int),
                        Convert.ToInt32(spatialClip.eulerAngle.y * Config.float2Int),
                        Convert.ToInt32(spatialClip.eulerAngle.z * Config.float2Int)
                    );
                    val.scale = Convert.ToInt32(spatialClip.scale * Config.float2Int);
                    actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
                }
                else if (clip is EditorSkillBreakEventClip skillBreakeventClip)
                {
                    var val = new SkillBreakEventActionData();
                    val.id = SkillActionDef.SKILL_BREAK_EVENT;
                    val.sframe = Convert.ToUInt32(clip.StartTime * GameDef.SP_DATA_FRAME);
                    val.eframe = Convert.ToUInt32(clip.EndTime * GameDef.SP_DATA_FRAME);
                    val.token = BreakTokenType.NONE;
                    if (skillBreakeventClip.joystick) val.token |= BreakTokenType.JOYSTICK;
                    if (skillBreakeventClip.recvhurt) val.token |= BreakTokenType.RECV_HURT;
                    if (skillBreakeventClip.recvcontrol) val.token |= BreakTokenType.RECV_CONTROL;
                    if (skillBreakeventClip.skillcast) val.token |= BreakTokenType.SKILL_CAST;
                    if (skillBreakeventClip.comboskillcast) val.token |= BreakTokenType.COMBO_SKILL_CAST;
                    actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
                }
            }

            SkillPipelineData spdata = new();
            spdata.id = Convert.ToUInt32(asset.name);
            spdata.length = length;
            spdata.actionIds = new ushort[actionDatas.Count];
            spdata.actionBytes = new byte[actionDatas.Count][];
            for (int i = 0; i < actionDatas.Count; ++i)
            {
                spdata.actionIds[i] = actionDatas[i].Item1;
                spdata.actionBytes[i] = actionDatas[i].Item2;
            }
            var bytes = MessagePackSerializer.Serialize(spdata);
            var path = $"{Application.dataPath.Replace("/Assets", "/")}{Location.skilldataPath}{asset.name}.bytes";
            if (File.Exists(path)) File.Delete(path);
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log($"写入文件：{path}");
            AssetDatabase.Refresh();
        }
    }
}
