using Goblin.Core;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// Behavior/行为
    /// </summary>
    public abstract class Behavior : Comp
    {
        /// <summary>
        /// 挂载的 Actor/实体
        /// </summary>
        public Actor actor { get; set; }
    }

    /// <summary>
    /// Behavior /行为, 包含 RIL 生成
    /// </summary>
    /// <typeparam name="T">RIL 翻译</typeparam>
    public abstract class Behavior<T> : Behavior where T : Translator, new()
    {
        public T translator { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            translator = AddComp<T>();
            translator.behavior = this;
            translator.Create();
        }
    }
}
