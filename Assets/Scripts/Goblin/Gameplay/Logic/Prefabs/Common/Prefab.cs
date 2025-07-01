using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Prefabs.Common
{
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
        /// 加载
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>预制创建器</returns>
        public Prefab Load(Stage stage)
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
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="state">预制创建器状态</param>
        public void Processing(ulong actor, PrefabInfoState state)
        {
            OnProcessing(actor, state);
        }

        /// <summary>
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="state">预制创建器状态</param>
        protected virtual void OnProcessing(ulong actor, PrefabInfoState state)
        {
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

        /// <summary>
        /// 设置职业生涯
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="bornpipelines">出生管线</param>
        /// <param name="deathpipelines">死亡管线</param>
        public void Career(ulong actor, List<int> bornpipelines, List<int> deathpipelines)
        {
            var career = stage.AddBehaviorInfo<CareerInfo>(actor);
            foreach (var bornpipeline in bornpipelines) career.bornpipelines.Add((uint)bornpipeline);
            foreach (var deathpipeline in deathpipelines) career.deathpipelines.Add((uint)deathpipeline);
            
            stage.silentmercy.Born(actor);
        }

        protected override void OnProcessing(ulong actor, PrefabInfoState state)
        {
            if (stage.SeekBehavior(actor, out Tag tag)) tag.Set(TAG_DEFINE.ACTOR_TYPE, type);
            OnProcessing(actor, (state as PrefabInfoState<T>).info);
        }

        /// <summary>
        /// 处理预制创建器逻辑
        /// </summary>
        /// <param name="actor">Actor</param>
        /// <param name="info">预制创建器信息</param>
        protected abstract void OnProcessing(ulong actor, T info);
    }
}