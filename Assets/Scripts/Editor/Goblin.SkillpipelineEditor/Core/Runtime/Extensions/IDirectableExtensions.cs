#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public static class IDirectableExtensions
    {
        public static float GetLength(this DirectableAsset directable)
        {
            return directable.EndTime - directable.StartTime;
        }

        public static float ToLocalTime(this DirectableAsset directable, float time)
        {
            return Mathf.Clamp(time - directable.StartTime, 0, directable.GetLength());
        }


        public static float ToLocalTimeUnclamped(this DirectableAsset directable, float time)
        {
            return time - directable.StartTime;
        }

        public static bool CanScale(this ActionClip directable)
        {
            var lengthProp = directable.GetType().GetProperty("Length", BindingFlags.Instance | BindingFlags.Public);
            return lengthProp != null && lengthProp.CanWrite && lengthProp.DeclaringType != typeof(ActionClip);
        }

        public static ActionClip GetPreviousSibling(this ActionClip directable)
        {
            if (directable.Parent != null)
            {
                return directable.Parent.Clips.LastOrDefault(d =>
                    d != directable && d.StartTime < directable.StartTime);
            }

            return null;
        }

        /// <summary>
        /// 返回父对象的下个同级
        /// </summary>
        /// <param name="directable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNextSibling<T>(this ActionClip directable) where T : ActionClip
        {
            return (T)GetNextSibling(directable);
        }

        public static ActionClip GetNextSibling(this ActionClip directable)
        {
            if (directable.Parent != null)
            {
                return directable.Parent.Clips.FirstOrDefault(d =>
                    d != directable && d.StartTime > directable.StartTime);
            }

            return null;
        }

        /// <summary>
        /// 返回剪辑的上一个循环长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static float GetPreviousLoopLocalTime(this ISubClipContainable clip)
        {
            float clipLength = 0;
            if (clip is DirectableAsset directableAsset)
            {
                clipLength = directableAsset.GetLength();
            }

            var loopLength = clip.SubClipLength / clip.SubClipSpeed;
            if (clipLength > loopLength)
            {
                var mod = (clipLength - clip.SubClipOffset) % loopLength;
                var aproxZero = Mathf.Abs(mod) < 0.01f;
                return clipLength - (aproxZero ? loopLength : mod);
            }

            return clipLength;
        }


        /// <summary>
        /// 返回剪辑的下一个循环长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static float GetNextLoopLocalTime(this ISubClipContainable clip)
        {
            float clipLength = 0;
            if (clip is DirectableAsset directableAsset)
            {
                clipLength = directableAsset.GetLength();
            }

            var loopLength = clip.SubClipLength / clip.SubClipSpeed;
            var mod = (clipLength - clip.SubClipOffset) % loopLength;
            var aproxZero = Mathf.Abs(mod) < 0.01f || Mathf.Abs(loopLength - mod) < 0.01f;
            return clipLength + (aproxZero ? loopLength : (loopLength - mod));
        }
    }
}
#endif