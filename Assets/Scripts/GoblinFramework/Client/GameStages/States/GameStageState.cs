using GoblinFramework.Client.Common;
using GoblinFramework.Client.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageState : FSMLockstepState<GameStage, GameStageState>
    {
        public override List<Type> PassStates => throw new NotImplementedException();

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }
    }
}
