using Assets.Scripts.GoblinFramework.Client.Gameplay.Comps;
using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay.Comps;
using GoblinFramework.Common.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay
{
    /// <summary>
    /// Theater 渲染剧场，所有渲染 Actor 的管理
    /// </summary>
    public class Theater : CPComp
    {
        private List<Actor> actorList = new List<Actor>();
        private Dictionary<int, Actor> actorDict = new Dictionary<int, Actor>();

        public CameraFollow CameraFollow;

        protected override void OnCreate()
        {
            base.OnCreate();
            CameraFollow = AddComp<CameraFollow>();
        }

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="ril">指令</param>
        /// <exception cref="Exception"></exception>
        public void Resolve<T>(T ril) where T : RIL
        {
            // 添加一个渲染 Actor
            if (RIL.RILType.RILAdd == ril.Type && false == actorDict.ContainsKey(ril.actorId)) AddActor(ril.actorId);

            var actor = GetActor(ril.actorId);
            if (null == actor) throw new Exception("can't find the actor. because check, plz let the riladd go as first.");

            // 渲染指令流水线
            actor.Resolve(ril);

            // 因为过一遍指令流水线来达到清理效果，最终清理 Actor，所以放到最后
            if (RIL.RILType.RILRmv == ril.Type && actorDict.ContainsKey(ril.actorId)) RmvActor(ril.actorId);
        }

        /// <summary>
        /// 查找演员
        /// </summary>
        /// <param name="actorId">ActorID，身份标识，来源于 Gameplay 逻辑部分的 ID</param>
        /// <returns>演员</returns>
        public Actor GetActor(int actorId)
        {
            actorDict.TryGetValue(actorId, out Actor actor);

            return actor;
        }

        /// <summary>
        /// 演员杀青
        /// </summary>
        /// <param name="actorId">ActorID，身份标识，来源于 Gameplay 逻辑部分的 ID</param>
        public void RmvActor(int actorId)
        {
            var actor = GetActor(actorId);

            actorDict.Remove(actorId);
            actorList.Remove(actor);

            RmvComp(actor);
        }

        /// <summary>
        /// 演员进组
        /// </summary>
        /// <param name="actorId">ActorID，身份标识，来源于 Gameplay 逻辑部分的 ID</param>
        public void AddActor(int actorId)
        {
            var actor = AddComp<Actor>((item) =>
            {
                item.actorId = actorId;
            });
            actorDict.Add(actorId, actor);
            actorList.Add(actor);
        }

        public new T AddComp<T>(Action<T> createAheadAction = null) where T : CPComp, new()
        {
            var comp = base.AddComp(createAheadAction);
            comp.Theater = this;

            return comp;
        }
    }
}
