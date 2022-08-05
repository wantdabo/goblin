using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackA : HoshiState
    {
        public override List<Type> PassStates => null;

        //protected override bool OnDetect()
        //{
        //    var release = Behavior.InputBehavior.GetInput(Behaviors.InputType.BA).release;

        //    return release;
        //}

        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateId = 3);
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnStateTick(int frame)
        {
            base.OnStateTick(frame);
        }
    }
}
