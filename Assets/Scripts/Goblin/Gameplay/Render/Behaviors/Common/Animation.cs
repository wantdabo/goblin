using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Render.Core;
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
        public string[] names { get; protected set; } = new string[StateDef.MAX_LAYER];

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

        private void OnTick(TickEvent e)
        {
            OnTick(e.tick);
        }

        private void OnModelChanged(ModelChangedEvent e)
        {
            CheckModelGo();
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        public abstract void Play(string name, byte layer = 0);

        /// <summary>
        /// Tick
        /// </summary>
        /// <param name="tick">tick</param>
        protected abstract void OnTick(float tick);

        /// <summary>
        /// 模型变化
        /// </summary>
        /// <param name="go">模型</param>
        protected abstract void OnModelChanged(GameObject go);
    }
}
