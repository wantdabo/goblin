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

            var inputBehavior = actor.GetBehavior<InputBehavior>();
            if (null == inputBehavior) throw new Exception($"this actor donot has inputbehavior {actorId}");

            inputBehavior.SetInput(inputType, input);
        }

        public void SendRIL<T>(T ril) where T : RIL
        {
            Engine.SendRIL(ril);
        }
    }
}
