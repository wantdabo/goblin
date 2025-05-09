using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.SkillPipelineEditor;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Goblin.Gameplay.Common.SkillData;
using Goblin.Gameplay.Common.SkillData.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using UnityEditor;
using UnityEngine;
using File = UnityEngine.Windows.File;
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
            uint length = Convert.ToUInt32(asset.Length * GAME_DEFINE.SP_DATA_FRAME);
            List<ActionClip> clips = new();
            foreach (var group in asset.groups)
            {
                foreach (var track in group.Tracks)
                {
                    clips.AddRange(track.Clips);
                }
            }
            //
            // List<(ushort, byte[])> actionDatas = new();
            // foreach (var clip in clips)
            // {
            //     (ushort, byte[]) actionData = default;
            //     if (clip is EditorAnimationClip animationClip)
            //     {
            //         var val = new AnimationData();
            //         val.id = SKILL_ACTION_DEFINE.ANIMATION;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.name = animationClip.animationClip.name;
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorEffectClip effectClip)
            //     {
            //         var val = new EffectData();
            //         val.id = SKILL_ACTION_DEFINE.EFFECT;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.res = Path.GetFileNameWithoutExtension(effectClip.res);
            //         val.position = new Vector3Data(
            //             Convert.ToInt32(effectClip.position.x * Config.Float2Int),
            //             Convert.ToInt32(effectClip.position.y * Config.Float2Int),
            //             Convert.ToInt32(effectClip.position.z * Config.Float2Int)
            //         );
            //         val.eulerAngles = new Vector3Data(
            //             Convert.ToInt32(effectClip.eulerAngles.x * Config.Float2Int),
            //             Convert.ToInt32(effectClip.eulerAngles.y * Config.Float2Int),
            //             Convert.ToInt32(effectClip.eulerAngles.z * Config.Float2Int)
            //         );
            //         val.scale = Convert.ToInt32(effectClip.scale * Config.Float2Int);
            //         val.binding = effectClip.binding;
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorSoundClip soundClip)
            //     {
            //         var val = new SoundData();
            //         val.id = SKILL_ACTION_DEFINE.SOUND;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.res = Path.GetFileNameWithoutExtension(soundClip.res);
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorSpatialClip spatialClip)
            //     {
            //         var val = new SpatialData();
            //         val.id = SKILL_ACTION_DEFINE.SPATIAL;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.position = new Vector3Data(
            //             Convert.ToInt32(spatialClip.position.x * Config.Float2Int),
            //             Convert.ToInt32(spatialClip.position.y * Config.Float2Int),
            //             Convert.ToInt32(spatialClip.position.z * Config.Float2Int)
            //         );
            //         val.scale = Convert.ToInt32(spatialClip.scale * Config.Float2Int);
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorBoxDetectionClip boxDetectionClip)
            //     {
            //         var val = new BoxDetectionData();
            //         val.id = SKILL_ACTION_DEFINE.BOX_DETECTION;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.position = new Vector3Data(
            //             Convert.ToInt32(boxDetectionClip.position.x * Config.Float2Int),
            //             Convert.ToInt32(boxDetectionClip.position.y * Config.Float2Int),
            //             Convert.ToInt32(boxDetectionClip.position.z * Config.Float2Int)
            //         );
            //         val.size = new Vector3Data(
            //             Convert.ToInt32(boxDetectionClip.size.x * Config.Float2Int),
            //             Convert.ToInt32(boxDetectionClip.size.y * Config.Float2Int),
            //             Convert.ToInt32(boxDetectionClip.size.z * Config.Float2Int)
            //         );
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorSphereDetectionClip sphereDetectionClip)
            //     {
            //         var val = new SphereDetectionData();
            //         val.id = SKILL_ACTION_DEFINE.SPHERE_DETECTION;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.position = new Vector3Data(
            //             Convert.ToInt32(sphereDetectionClip.position.x * Config.Float2Int),
            //             Convert.ToInt32(sphereDetectionClip.position.y * Config.Float2Int),
            //             Convert.ToInt32(sphereDetectionClip.position.z * Config.Float2Int)
            //         );
            //         val.radius = Convert.ToInt32(sphereDetectionClip.radius * Config.Float2Int);
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorBreakTokenEventClip skillBreakeventClip)
            //     {
            //         var val = new BreakTokenEventData();
            //         val.id = SKILL_ACTION_DEFINE.BREAK_TOKEN_EVENT;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.token = BREAK_TOKEN_DEFINE.NONE;
            //         if (skillBreakeventClip.joystick) val.token |= BREAK_TOKEN_DEFINE.JOYSTICK;
            //         if (skillBreakeventClip.recvhurt) val.token |= BREAK_TOKEN_DEFINE.RECV_HURT;
            //         if (skillBreakeventClip.recvcontrol) val.token |= BREAK_TOKEN_DEFINE.RECV_CONTROL;
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorBreakFramesEventClip skillBreakFramesEventClip)
            //     {
            //         var val = new BreakFramesEventData();
            //         val.id = SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.selfbreakframes = Convert.ToUInt32(skillBreakFramesEventClip.selfbreakframes);
            //         val.targetbreakframes = Convert.ToUInt32(skillBreakFramesEventClip.targetbreakframes);
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorBuffTriggerEventClip buffTriggerEventClip)
            //     {
            //         var val = new BuffTriggerEventData();
            //         val.id = SKILL_ACTION_DEFINE.BUFF_TRIGGER_EVENT;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.buffid = Convert.ToUInt32(buffTriggerEventClip.buffid);
            //         val.triggerself = buffTriggerEventClip.triggerself;
            //         val.triggertarget = buffTriggerEventClip.triggertarget;
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            //     else if (clip is EditorBuffStampEventClip buffStampEventClip)
            //     {
            //         var val = new BuffStampEventData();
            //         val.id = SKILL_ACTION_DEFINE.BUFF_STAMP_EVENT;
            //         val.sframe = Convert.ToUInt32(clip.StartTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.eframe = Convert.ToUInt32(clip.EndTime * GAME_DEFINE.SP_DATA_FRAME);
            //         val.buffid = Convert.ToUInt32(buffStampEventClip.buffid);
            //         val.stampself = buffStampEventClip.stampself;
            //         val.stamptarget = buffStampEventClip.stamptarget;
            //         val.layer = Convert.ToUInt32(buffStampEventClip.layer);
            //         actionDatas.Add((val.id, MessagePackSerializer.Serialize(val)));
            //     }
            // }
            //
            // SkillPipelineData spdata = new();
            // spdata.id = Convert.ToUInt32(asset.name);
            // spdata.length = length;
            // spdata.actionIds = new ushort[actionDatas.Count];
            // spdata.actionBytes = new byte[actionDatas.Count][];
            // for (int i = 0; i < actionDatas.Count; ++i)
            // {
            //     spdata.actionIds[i] = actionDatas[i].Item1;
            //     spdata.actionBytes[i] = actionDatas[i].Item2;
            // }
            // var bytes = MessagePackSerializer.Serialize(spdata);
            // var path = $"{Application.dataPath.Replace("/Assets", "/")}{Location.skilldatapath}{asset.name}.bytes";
            // if (File.Exists(path)) File.Delete(path);
            // System.IO.File.WriteAllBytes(path, bytes);
            // Debug.Log($"写入文件：{path}");
            // AssetDatabase.Refresh();
        }
    }
}
