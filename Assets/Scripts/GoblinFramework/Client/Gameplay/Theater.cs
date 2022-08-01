using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay
{
    /// <summary>
    /// 剧场，所有 Actor 的管理
    /// </summary>
    public class Theater : CComp
    {
        private List<Actor> actorList = new List<Actor>();
        private Dictionary<int, Actor> actorDict = new Dictionary<int, Actor>();

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="cmd">指令</param>
        /// <exception cref="Exception"></exception>
        public void Resolve<T>(T cmd) where T : SyncCmd
        {
            if (SyncCmd.CType.SyncAddCmd == cmd.Type && false == actorDict.ContainsKey(cmd.actorId)) AddActor(cmd.actorId);
            if (SyncCmd.CType.SyncRmvCmd == cmd.Type && actorDict.ContainsKey(cmd.actorId)) RmvActor(cmd.actorId);

            var actor = GetActor(cmd.actorId);
            if (null == actor) throw new Exception("can't find the actor. because check, plz let the SyncAddCmd go as first.");

            actor.Resolve(cmd);
        }

        /// <summary>
        /// 查找演员
        /// </summary>
        /// <param name="actorId">演员 ID，来源于 Gameplay 逻辑部分的 ID</param>
        /// <returns>演员</returns>
        public Actor GetActor(int actorId)
        {
            actorDict.TryGetValue(actorId, out Actor actor);

            return actor;
        }

        /// <summary>
        /// 演员杀青
        /// </summary>
        /// <param name="actorId">ActorID，身份标识</param>
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
        /// <param name="actorId">ActorID，身份标识</param>
        public void AddActor(int actorId)
        {
            var actor = AddComp<Actor>((item) =>
            {
                item.Theater = this;
                item.actorId = actorId;
            });
            actorDict.Add(actorId, actor);
            actorList.Add(actor);
        }
    }
}
