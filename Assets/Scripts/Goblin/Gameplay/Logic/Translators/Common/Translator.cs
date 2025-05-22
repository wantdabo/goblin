using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
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
            OnLoad();
            
            return this;
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            OnUnload();
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
        /// 加载
        /// </summary>
        protected virtual void OnLoad()
        {
        }
        
        /// <summary>
        /// 卸载
        /// </summary>
        protected virtual void OnUnload()
        {
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
        /// 计算哈希值
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <returns>BehaviorInfo 哈希值</returns>
        protected virtual int OnCalcHashCode(T info)
        {
            return info.GetHashCode();
        }

        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        protected override void OnRIL(BehaviorInfo info)
        {
            var result = CacheHashCode(info);
            if (false == result.diffed) return;
            GenRIL(info, result.hashcode);
        }
        
        /// <summary>
        /// 缓存渲染指令的哈希值
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <returns>(diffed : 是否有差异, hashcode : 哈希值)</returns>
        protected (bool diffed, int hashcode) CacheHashCode(BehaviorInfo info)
        {
            int hashcode = OnCalcHashCode(info as T);
            if (stage.rilsync.Query(info.id, id).Equals(hashcode)) return (false, hashcode);
            stage.rilsync.CacheHashCode(info.id, id, hashcode);

            return (true, hashcode);
        }
        
        /// <summary>
        /// 生成渲染指令
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <param name="hashcode">哈希值</param>
        protected void GenRIL(BehaviorInfo info, int hashcode)
        {
            var ril = RILCache.Ensure<E>();
            ril.Ready(info.id, hashcode);
            OnRIL(info as T, ril);
            stage.rilsync.Send(ril);
        }

        /// <summary>
        /// 渲染指令处理
        /// </summary>
        /// <param name="info">BehaviorInfo</param>
        /// <param name="ril">渲染指令</param>
        protected abstract void OnRIL(T info, E ril);
    }
}