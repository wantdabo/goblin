using GoblinFramework.Gameplay.Actors;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Theaters
{
    public class Theater : Actor
    {
        private Dictionary<int, List<RIL>> rilsDict = new Dictionary<int, List<RIL>>();
        protected override void OnCreate()
        {
            Actor = this;
            Theater = this;

            base.OnCreate();

            AddActor<Actors.Hoshi.HoshiActor>();
        }

        public void SendRIL<T>(T ril) where T : RIL
        {
            if(false == rilsDict.ContainsKey(Engine.TickEngine.Frame)) rilsDict.Add(Engine.TickEngine.Frame, new List<RIL>());
            rilsDict.TryGetValue(Engine.TickEngine.Frame, out var rils);
            rils.Add(ril);
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
