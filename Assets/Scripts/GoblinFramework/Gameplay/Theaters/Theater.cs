using GoblinFramework.Gameplay.Actors;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Theaters
{
    public class Theater : Actor
    {
        private Dictionary<int, List<SyncCmd>> syncCmdsDict = new Dictionary<int, List<SyncCmd>>();
        protected override void OnCreate()
        {
            Actor = this;
            Theater = this;

            base.OnCreate();

            AddActor<Actors.Hoshi.HoshiActor>();
        }

        public void SendSyncCmd<T>(T cmd) where T : SyncCmd
        {
            if(false == syncCmdsDict.ContainsKey(Engine.TickEngine.Frame)) syncCmdsDict.Add(Engine.TickEngine.Frame, new List<SyncCmd>());
            syncCmdsDict.TryGetValue(Engine.TickEngine.Frame, out var syncCmdList);
            syncCmdList.Add(cmd);
        }

        private int actorIdGen = -1;
        public int NewActorId
        {
            get
            {
                actorIdGen += 1;

                return actorIdGen;
            }
        }
    }
}
