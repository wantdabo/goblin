using Goblin.Common;
using Goblin.Common.GameRes;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    }
}
