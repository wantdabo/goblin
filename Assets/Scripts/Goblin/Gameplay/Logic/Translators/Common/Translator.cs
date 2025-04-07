using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Translators.Common
{
    /// <summary>
    /// 渲染指令翻译器
    /// </summary>
    public abstract class Translator
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>翻译器</returns>
        public Translator Load(Stage stage)
        {
            this.stage = stage;

            return this;
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            this.stage = null;
        }

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        public void Translate(BehaviorInfo info)
        {
            OnRIL(info);
        }

        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        protected virtual void OnRIL(BehaviorInfo info)
        {
        }
    }
    
    /// <summary>
    /// 渲染指令翻译器
    /// </summary>
    /// <typeparam name="T">BehaviorInfo 类型</typeparam>
    public abstract class Translator<T> : Translator where T : BehaviorInfo
    {
        protected override void OnRIL(BehaviorInfo info)
        {
            base.OnRIL(info);
            OnRIL((T)info);
        }
        
        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        protected abstract void OnRIL(T info);
    }
}