using Goblin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Common.Sounds
{
    /// <summary>
    /// 音效
    /// </summary>
    public class SoundInfo : Comp
    {
        /// <summary>
        /// 资源名
        /// </summary>
        public string res { get; private set; }
        /// <summary>
        /// 正在播放中
        /// </summary>
        public bool playing { get; private set; }
        /// <summary>
        /// 音效 Go
        /// </summary>
        public GameObject go { get; private set; }
        /// <summary>
        /// 音效播放器
        /// </summary>
        public AudioSource audio { get; private set; }

        /// <summary>
        /// 初始化音效
        /// </summary>
        /// <param name="res">资源名</param>
        /// <param name="go">音效 Go</param>
        public void Initialize(string res, GameObject go)
        {
            this.res = res;
            this.go = go;
            this.audio = engine.u3dkit.GetNode<AudioSource>(this.go);
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            playing = true;
            go.SetActive(true);
            audio.Play();
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            playing = false;
            go.SetActive(false);
            audio.Stop();
        }
    }
}
