using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiIdle : HoshiState
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateId = 1);
        }
    }
}
