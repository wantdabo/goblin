using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Focus;
using Goblin.Gameplay.Render.Focus.Cameras;
using Goblin.Gameplay.Render.Focus.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Core
{
    public struct AddActorEvent : IEvent
    {
        public Actor actor { get; set; }
    }
    
    public struct RmvActorEvent : IEvent
    {
        public Actor actor { get; set; }
    }
    
    public class Stage : Comp
    {
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Ticker/时间驱动器
        /// </summary>
        public Ticker ticker { get; private set; }
        /// <summary>
        /// RIL/ 渲染指令同步
        /// </summary>
        public RILSync rilsync { get; private set; }
        /// <summary>
        /// 专注/焦点
        /// </summary>
        public Foc foc { get; private set; }
        /// <summary>
        /// Actor 列表
        /// </summary>
        public List<Actor> actors { get; private set; } = new();
        /// <summary>
        /// Actor 字典
        /// </summary>
        private Dictionary<uint, Actor> actorDict = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
            
            rilsync = AddComp<RILSync>();
            rilsync.stage = this;
            rilsync.Create();

            foc = AddComp<Foc>();
            foc.stage = this;
            foc.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = actors.Count - 1; i >= 0; i--) actors[i].Destroy();
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="tick">tick</param>
        public void Tick(float tick)
        {
            ticker.Tick(tick);
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
            return actorDict.GetValueOrDefault(id);
        }
        
        /// <summary>
        /// 添加 Actor
        /// </summary>
        /// <returns>Actor</returns>
        public Actor AddActor(uint id)
        {
            var actor = AddComp<Actor>();
            actor.id = id;
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
        private void RmvActor(uint id)
        {
            var actor = GetActor(id);
            if (null == actor) return;
        
            RmvActor(actor);
        }
        
        /// <summary>
        /// 根据 Actor 实例移除 Actor
        /// </summary>
        /// <param name="actor">Actor 实例</param>
        private void RmvActor(Actor actor)
        {
            if (false == actors.Contains(actor)) return;
        
            actors.Remove(actor);
            actorDict.Remove(actor.id);
            eventor.Tell(new RmvActorEvent { actor = actor });
        }
    }
}
