using Goblin.Gameplay.Common.Defines;
using Goblin.SkillPipelineEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
        }
    }
}
