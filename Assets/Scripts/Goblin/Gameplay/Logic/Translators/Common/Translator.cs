using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;

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
            stage = null;
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
        protected abstract void OnRIL(BehaviorInfo info);
    }
    
    /// <summary>
    /// 渲染指令翻译器
    /// </summary>
    /// <typeparam name="T">BehaviorInfo 类型</typeparam>
    /// <typeparam name="E">RIL 类型</typeparam>
    public abstract class Translator<T, E> : Translator where T : BehaviorInfo where E : IRIL, new()
    {
        /// <summary>
        /// 渲染指令 ID
        /// </summary>
        protected abstract ushort id { get; }
        
        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        protected override void OnRIL(BehaviorInfo info)
        {
            int hashcode = CalcHashCode(info as T, info.GetHashCode());
            if (stage.rilsync.Query(info.id, id).Equals(hashcode)) return;
            
            var ril = ObjectCache.Ensure<E>();
            ril.Ready(info.id, hashcode);
            OnRIL(info as T, ril);
            stage.rilsync.Send(ril);
        }
        
        /// <summary>
        /// 计算渲染指令哈希值
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <param name="hashcode">BehaviorInfo 哈希值</param>
        /// <returns>BehaviorInfo 哈希值</returns>
        protected virtual int CalcHashCode(T info, int hashcode)
        {
            return hashcode;
        }
        
        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnRIL(T info, E ril);
    }
}