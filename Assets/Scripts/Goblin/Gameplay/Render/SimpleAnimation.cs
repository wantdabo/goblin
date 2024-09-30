using Goblin.Gameplay.Render.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Gameplay.Render
{
    public class SimpleAnimation : ModelAnimation
    {
        private Animator animator;

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<PlayAnimEvent>(OnPlayAnim);
            actor.eventor.Listen<LoadModelStatusChangedEvent>(OnLoadModelStatusChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<PlayAnimEvent>(OnPlayAnim);
            actor.eventor.UnListen<LoadModelStatusChangedEvent>(OnLoadModelStatusChanged);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="e">播放动画的参数</param>
        /// <exception cref="Exception">动画状态机层数超过最大上限</exception>
        private void OnPlayAnim(PlayAnimEvent e)
        {
            if (e.layer >= animNames.Length) throw new Exception($"max layer need < {animNames.Length}");
            if (loops[e.layer] && animNames[e.layer] == e.animName) return;
            animNames[e.layer] = e.animName;
            loops[e.layer] = e.loop;

            if (null == animator) return;
            animator.Play(e.animName);
        }

        /// <summary>
        /// 模型加载通知
        /// </summary>
        /// <param name="e">模型加载状态数据</param>
        private void OnLoadModelStatusChanged(LoadModelStatusChangedEvent e)
        {
            if (LoadModelStatus.Start == e.status)
            {
                for (int i = 0; i < animNames.Length; i++)
                {
                    animNames[i] = "";
                }
            }
            else
            {
                var model = actor.GetBehavior<ModelRender>().model;
                animator = model.GetComponentInChildren<Animator>();
                for (int i = 0; i < animNames.Length; i++)
                {
                    if (string.IsNullOrEmpty(animNames[i])) continue;
                    OnPlayAnim(new PlayAnimEvent { animName = animNames[i], loop = loops[i], layer = i });
                }
            }
        }
    }
}