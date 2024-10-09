using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Translations.Common
{
    /// <summary>
    /// 翻译 /RIL
    /// </summary>
    public abstract class Translator : Comp
    {
        /// <summary>
        /// Behavior/行为
        /// </summary>
        public Behavior behavior { get; set; }
    }

    /// <summary>
    /// 翻译 /RIL
    /// </summary>
    /// <typeparam name="T">Behavior/行为类型</typeparam>
    public abstract class Translator<T> : Translator where T : Behavior
    {
        /// <summary>
        /// Behavior/行为
        /// </summary>
        public new T behavior { get => base.behavior as T; }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // 预生成一次
            OnRIL();
            behavior.actor.stage.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            behavior.actor.stage.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            OnRIL();
        }

        /// <summary>
        /// 生成 RIL
        /// </summary>
        /// <returns>RIL</returns>
        protected abstract void OnRIL();
    }
}
