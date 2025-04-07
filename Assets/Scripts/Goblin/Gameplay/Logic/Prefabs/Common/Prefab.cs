using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Prefabs.Common
{
    /// <summary>
    /// 预制创建器信息
    /// </summary>
    public interface IPrefabInfo
    {
        
    }
    
    /// <summary>
    /// 预制创建器, 创建设定好的 Actor
    /// </summary>
    public abstract class Prefab
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>预制创建器</returns>
        public Prefab Initialize(Stage stage)
        {
            this.stage = this.stage;

            return this;
        }

        /// <summary>
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="info">预制创建器信息</param>
        /// <returns>Actor</returns>
        public Actor Processing(Actor actor, IPrefabInfo info)
        {
            return OnProcessing(actor, info);
        }

        /// <summary>
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="info">预制创建器信息</param>
        /// <returns>Actor</returns>
        protected virtual Actor OnProcessing(Actor actor, IPrefabInfo info)
        {
            return default;
        }
    }
    
    /// <summary>
    /// 预制创建器, 创建设定好的 Actor
    /// </summary>
    /// <typeparam name="T">预制创建器信息类型</typeparam>
    public abstract class Prefab<T> : Prefab where T : IPrefabInfo
    {
        /// <summary>
        /// 预制类型
        /// </summary>
        public abstract byte type { get; }
        
        protected override Actor OnProcessing(Actor actor, IPrefabInfo info)
        {
            if (actor.SeekBehavior(out Tag tag))
            {
                tag.Set(TAG_DEFINE.ACTOR_TYPE, type);
            }
            
            OnProcessing(actor, (T)info);

            return actor;
        }

        /// <summary>
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="info">预制创建器信息</param>
        protected abstract void OnProcessing(Actor actor, T info);
    }
}