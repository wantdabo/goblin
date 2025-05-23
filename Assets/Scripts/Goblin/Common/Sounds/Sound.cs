﻿using Goblin.Core;
using UnityEngine;

namespace Goblin.Common.Sounds
{
    /// <summary>
    /// 音效
    /// </summary>
    public class Sound : Comp
    {
        public static GameObject root = new("Sound");
        static Sound()
        {
            root.transform.SetParent(GameObject.Find("Gameplay").transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 卸载音效
        /// </summary>
        /// <param name="sound">音效</param>
        public void Unload(SoundInfo sound)
        {
            if (null == sound) return;
            sound.Stop();
            RecycleSound(sound);
        }

        /// <summary>
        /// 加载音效
        /// </summary>
        /// <param name="res">音效资源名</param>
        /// <returns>音效</returns>
        public SoundInfo Load(string res)
        {
            var sound = ObjectPool.Get<SoundInfo>($"SOUND_KEY{res}");
            if (null == sound)
            {
                sound = AddComp<SoundInfo>();
                var go = engine.gameres.location.LoadSoundSync(res);
                go.transform.SetParent(root.transform, false);
                sound.Initialize(res, go);
                sound.Create();
            }
            
            if (sound.audio.loop) return sound;
            
            engine.ticker.Timing((t) =>
            {
                sound.Stop();
                Unload(sound);
            }, sound.audio.clip.length, 1);
            
            return sound;
        }

        /// <summary>
        /// 回收音效
        /// </summary>
        /// <param name="sound">音效</param>
        private void RecycleSound(SoundInfo sound)
        {
            if (null == sound) return;

            ObjectPool.Set(sound, $"SOUND_KEY{sound.res}");
        }
    }
}