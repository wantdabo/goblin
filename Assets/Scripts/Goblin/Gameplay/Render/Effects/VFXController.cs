using Goblin.Gameplay.Common.Defines;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Effects
{
    /// <summary>
    /// 特效控制器
    /// </summary>
    [DisallowMultipleComponent]
    public class VFXController : MonoBehaviour
    {
        /// <summary>
        /// 动画状态机集合
        /// </summary>
        [HideInInspector] public Animator[] animators;

        /// <summary>
        /// 粒子系统集合
        /// </summary>
        [HideInInspector] public ParticleSystem[] pss;

        /// <summary>
        /// 持续时间
        /// </summary>
        [HideInInspector] public float duration;
        
        /// <summary>
        /// 特效管理器
        /// </summary>
        public VisualEffect vfx { get; set; }

        /// <summary>
        /// 播放中
        /// </summary>
        public bool playing { get; private set; } = false;
        
        /// <summary>
        /// 开启插值递进
        /// </summary>
        private bool lerp { get; set; }
        /// <summary>
        /// 插值递进目标
        /// </summary>
        private float lerpt { get; set; }
        
        /// <summary>
        /// 延迟 Active 计时器
        /// </summary>
        private uint delayTimingId { get; set; }
        /// <summary>
        /// 自动停止计时器
        /// </summary>
        private uint autoStopTimingId { get; set; }

        /// <summary>
        /// 驱动更新特效
        /// </summary>
        /// <param name="tick">tick</param>
        public void OnTick(float tick)
        {
            if (false == playing) return;
            if (lerp)
            {
                if (tick > lerpt) return;
                lerpt -= tick;
            }

            foreach (var ps in pss) if (ps.gameObject.activeInHierarchy) ps.Simulate(tick, true, false);
            foreach (var animator in animators) if (animator.gameObject.activeInHierarchy) animator.Update(tick);
        }
        
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (false == playing) return;
            if (null == vfx) return;
            playing = false;
            lerpt = 0f;
            vfx.stage.ticker.StopTimer(delayTimingId);
            gameObject.SetActive(false);
            foreach (var animator in animators) if (animator.isActiveAndEnabled) animator.Play(string.Empty);
            foreach (var ps in pss)
            {
                ps.Stop();
                ps.Clear();
            }

            vfx.stage.ticker.StopTimer(autoStopTimingId);
            vfx.UnloadVFX(this);
        }  

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            lerp = false;
            Do();
        }
    
        /// <summary>
        /// 播放 (tick 插值递进)
        /// </summary>
        /// <param name="tick">递进 tick</param>
        public void Play(float tick)
        {
            this.lerp = true;
            this.lerpt += tick;
            if (playing) return;
            Do();
        }
        
        /// <summary>
        /// 播放 (持续时间)
        /// </summary>
        private void Do()
        {
            playing = true;
            delayTimingId = vfx.stage.ticker.Timing((t) =>
            {
                gameObject.SetActive(true);
                foreach (var animator in animators) if (animator.gameObject.activeInHierarchy) animator.Play(string.Empty);
                foreach (var ps in pss) if (ps.gameObject.activeInHierarchy) ps.Play();
                if (duration <= 0) return;
                autoStopTimingId = vfx.stage.ticker.Timing((t) => Stop(), duration, 1);
            }, GAME_DEFINE.SP_DATA_TICK, 1);
        }
    }
}
