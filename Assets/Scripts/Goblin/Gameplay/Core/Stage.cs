using Goblin.Common;
using Goblin.Common.Network;
using Goblin.Gameplay.Actors;
using Goblin.Gameplay.Common;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TrueSync;

namespace Goblin.Gameplay.Core
{
    #region Events
    /// <summary>
    /// 添加 Actor/实体事件
    /// </summary>
    public struct AddActorEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor;
    }

    /// <summary>
    /// 移除 Actor/实体事件
    /// </summary>
    public struct RmvActorEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor;
    }

    #endregion

    /// <summary>
    /// Stage/关卡
    /// </summary>
    public class Stage : Actor
    {
        /// <summary>
        /// Actor 自增 ID
        /// </summary>
        private uint incrementId = 0;

        /// <summary>
        /// 座位
        /// </summary>
        public uint seat { get; private set; }

        /// <summary>
        /// 确定性，Ticker/时间驱动器
        /// </summary>
        public FPTicker ticker { get; private set; }
        /// <summary>
        /// 确定性，随机器
        /// </summary>
        public FPRandom random { get; private set; }

        /// <summary>
        /// Actor 列表
        /// </summary>
        public List<Actor> actors { get; private set; } = new();

        /// <summary>
        /// 玩家列表
        /// </summary>
        public Player[] players { get; private set; }

        /// <summary>
        /// Actor 字典
        /// </summary>
        private Dictionary<uint, Actor> actorDict = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            ticker = AddComp<FPTicker>();
            ticker.Create();

            random = AddComp<FPRandom>();
            random.Initial(19491001);
            random.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = actors.Count - 1; i >= 0; i--) actors[i].Destroy();
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="frameInfo">帧信息</param>
        public void Gaming()
        {
            ticker.Tick();
        }

        /// <summary>
        /// 获得 Actor（根据泛型转型）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">id</param>
        /// <returns>Actor</returns>
        public T GetActor<T>(uint id) where T : Actor
        {
            var actor = GetActor(id);

            if (null != actor) return actor as T;

            return null;
        }

        /// <summary>
        /// 获得 Actor
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Actor</returns>
        public Actor GetActor(uint id)
        {
            if (actorDict.TryGetValue(id, out var actor)) return actor;

            return null;
        }

        /// <summary>
        /// 添加 Actor
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>Actor</returns>
        public T AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>();
            actor.id = ++incrementId;
            actor.stage = this;
            actors.Add(actor);
            actorDict.Add(actor.id, actor);
            eventor.Tell(new AddActorEvent { actor = actor });

            return actor;
        }

        /// <summary>
        /// 根据 ID 移除 Actor
        /// </summary>
        /// <param name="id">ID</param>
        public void RmvActor(uint id)
        {
            var actor = GetActor(id);
            if (null == actor) return;

            RmvActor(actor);
        }

        /// <summary>
        /// 根据 Actor 实例移除 Actor
        /// </summary>
        /// <param name="actor">Actor 实例</param>
        public void RmvActor(Actor actor)
        {
            if (false == actors.Contains(actor)) return;

            actors.Remove(actor);
            actorDict.Remove(actor.id);
            eventor.Tell(new RmvActorEvent { actor = actor });
        }
    }
}