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
            int hashcode = CalcHashCode(info as T, info.GetHashCode());
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

    /// <summary>
    /// 渲染指令翻译器
    /// </summary>
    /// <typeparam name="T">BehaviorInfo 类型</typeparam>
    /// <typeparam name="E">RIL 类型</typeparam>
    /// <typeparam name="D">RIL_DIFF 类型</typeparam>
    public abstract class Translator<T, E, D> : Translator<T, E> where T : BehaviorInfo where E : IRIL, new() where D : IRIL_DIFF, new()
    {
        protected override void OnRIL(BehaviorInfo info)
        {
            var result = CacheHashCode(info);
            if (false == result.diffed) return;
            
            var deldiff = RILCache.Ensure<D>();
            var newdiff = RILCache.Ensure<D>();
            deldiff.Ready(info.id, RIL_DEFINE.DIFF_DEL);
            newdiff.Ready(info.id, RIL_DEFINE.DIFF_NEW);
            
            // Diff 检查
            var diff = OnDiff(info as T, deldiff, newdiff);
            if (diff.deldiff)
            {
                // TODO 发送 DEL_DIFF
            }
            else
            {
                deldiff.Reset();
                RILCache.Set(deldiff);
            }

            if (diff.newdiff)
            {
                // TODO 发送 NEW_DIFF
            }
            else
            {
                newdiff.Reset();
                RILCache.Set(newdiff);
            }

            if (diff.deldiff || diff.newdiff) return;
            
            GenRIL(info, result.hashcode);
        }

        protected abstract (bool deldiff, bool newdiff) OnDiff(T info, D deldiff, D newdiff);
    }
}