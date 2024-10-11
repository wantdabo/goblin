using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Render.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors.Common
{
    /// <summary>
    /// 动画播放
    /// </summary>
    public abstract class Animation : Behavior
    {
        /// <summary>
        /// 当前播放的动画名
        /// </summary>
        public string[] names { get; private set; } = new string[StateDef.MAX_LAYER];
        public Queue<(string, byte)> animseqs { get; private set; } = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            CheckModelGo();
            actor.stage.ticker.eventor.Listen<TickEvent>(OnTick);
            actor.eventor.Listen<ModelChangedEvent>(OnModelChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<TickEvent>(OnTick);
            actor.eventor.UnListen<ModelChangedEvent>(OnModelChanged);
        }

        private void CheckModelGo()
        {
            var model = actor.EnsureBehavior<Model>();
            if (null == model.go) return;
            OnModelChanged(model.go);
        }
        
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        public void Play(string name, byte layer = 0)
        {
            names[layer] = name;
            OnPlay(name, layer);
        }
        
        /// <summary>
        /// 清空动画序列
        /// </summary>
        public void EmptySequeue()
        {
            animseqs.Clear();
        }
        
        /// <summary>
        /// 播放动画序列
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        public void PlaySequeue(string name, byte layer = 0)
        {
            animseqs.Enqueue((name, layer));
        }

        private void OnTick(TickEvent e)
        {
            OnTick(e.tick);
            if (0 == animseqs.Count) return;
            if (false == OnNextSequeue()) return;
            var anim = animseqs.Dequeue();
            Play(anim.Item1, anim.Item2);
        }

        private void OnModelChanged(ModelChangedEvent e)
        {
            CheckModelGo();
        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="tick">tick</param>
        protected abstract void OnTick(float tick);

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        protected abstract void OnPlay(string name, byte layer = 0);
        
        /// <summary>
        /// 播放下一个动画
        /// </summary>
        /// <returns>YES/NO</returns>
        protected virtual bool OnNextSequeue()
        {
            return false;
        }

        /// <summary>
        /// 模型变化
        /// </summary>
        /// <param name="go">模型</param>
        protected abstract void OnModelChanged(GameObject go);
    }
}
