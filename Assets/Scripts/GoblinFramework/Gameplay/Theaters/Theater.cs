using GoblinFramework.Gameplay.Actors;
using GoblinFramework.Gameplay.Behaviors;
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

        public void SetInput(int actorId, InputType inputType, Input input)
        {
            var actor = GetActor(actorId);
            if (null == actor) throw new Exception($"actor not found {actorId}");

            var inputBehavior = GetBehavior<InputBehavior>();
            if (null == inputBehavior) throw new Exception($"this actor donot has inputbehavior {actorId}");

            inputBehavior.SetInput(inputType, input);
        }

        public void SendRIL<T>(T ril) where T : RIL
        {
            if (false == rilsDict.ContainsKey(Engine.TickEngine.Frame)) rilsDict.Add(Engine.TickEngine.Frame, new List<RIL>());
            rilsDict.TryGetValue(Engine.TickEngine.Frame, out var rils);
            rils.Add(ril);

            Engine.SendRIL(ril);
        }
    }
}
