using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Attributes.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Physics.Common;
using System.Collections.Generic;
using TrueSync;
using TrueSync.Physics2D;
using TrueSync.Physics3D;
using World = TrueSync.Physics3D.World;

namespace Goblin.Gameplay.Logic.Core
{
    #region Events
    /// <summary>
    /// 添加 Actor/实体事件
    /// </summary>
    public struct ActorDeadEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor { get; set; }
    }

    /// <summary>
    /// 移除 Actor/实体事件
    /// </summary>
    public struct ActorLiveEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor { get; set; }
    }
    #endregion

    /// <summary>
    /// Stage/关卡
    /// </summary>
    public class Stage : Comp
    {
        /// <summary>
        /// Actor 自增 ID
        /// </summary>
        private uint incrementId = 0;
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// 确定性，Ticker/时间驱动器
        /// </summary>
        public FPTicker ticker { get; private set; }
        /// <summary>
        /// 确定性，随机器
        /// </summary>
        public FPRandom random { get; private set; }
        /// <summary>
        /// 数值计算器
        /// </summary>
        public Calculator calc { get; private set; }
        /// <summary>
        /// RIL/ 渲染指令同步
        /// </summary>
        public RILSync rilsync { get; private set; }
        /// <summary>
        /// 物理
        /// </summary>
        public Phys phys { get; private set; }
        /// <summary>
        /// Actor 列表
        /// </summary>
        private List<Actor> actors { get; set; } = new();
        /// <summary>
        /// Actor 字典
        /// </summary>
        private Dictionary<uint, Actor> actordict { get; set; } = new();
        /// <summary>
        /// 活着的 ActorID 列表
        /// </summary>
        public List<uint> lives { get; private set; } = new();
        /// <summary>
        /// 死亡的 ActorID 列表
        /// </summary>
        public List<uint> deads { get; private set; } = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();

            ticker = AddComp<FPTicker>();
            ticker.Create();

            random = AddComp<FPRandom>();
            random.Initial(19491001);
            random.Create();

            calc = AddComp<Calculator>();
            calc.Create();

            rilsync = AddComp<RILSync>();
            rilsync.stage = this;
            rilsync.Create();

            phys = AddComp<Phys>();
            phys.stage = this;
            phys.Initialize<CollisionSystemPersistentSAP>();
            phys.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = actors.Count - 1; i >= 0; i--) actors[i].Destroy();
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        public void Tick()
        {
            ticker.Tick();
            
            foreach (uint dead in deads)
            {
                var deada = RmvActor(dead);
                deada.Destroy();
            }
            deads.Clear();
        }

        /// <summary>
        /// Actor 存活
        /// </summary>
        /// <param name="id">ActorID</param>
        public void Live(uint id)
        {
            if (deads.Contains(id)) return;

            var actor = GetActor(id);
            if (null == actor) return;

            eventor.Tell(new ActorLiveEvent { actor = actor });

            lives.Add(id);
        }

        /// <summary>
        /// Actor 死亡
        /// </summary>
        /// <param name="id">ActorID</param>
        public void Dead(uint id)
        {
            if (false == lives.Contains(id)) return;
            var actor = GetActor(id);
            if (null == actor) return;

            eventor.Tell(new ActorDeadEvent { actor = actor });

            deads.Add(id);
        }

        /// <summary>
        /// 获得 Actor
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Actor</returns>
        public Actor GetActor(uint id)
        {
            return actordict.GetValueOrDefault(id);
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
            actordict.Add(actor.id, actor);

            return actor;
        }

        /// <summary>
        /// 根据 ID 移除 Actor
        /// </summary>
        /// <param name="id">ID</param>
        private Actor RmvActor(uint id)
        {
            var actor = GetActor(id);
            if (null == actor) return null;

            RmvActor(actor);

            return actor;
        }

        /// <summary>
        /// 根据 Actor 实例移除 Actor
        /// </summary>
        /// <param name="actor">Actor 实例</param>
        private void RmvActor(Actor actor)
        {
            if (false == actors.Contains(actor)) return;

            actors.Remove(actor);
            actordict.Remove(actor.id);
        }
    }
}
