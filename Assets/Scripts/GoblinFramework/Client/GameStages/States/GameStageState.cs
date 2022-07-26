using GoblinFramework.Core.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageState : FSMLockstepState<CGEngine, GameStage, GameStageState>
    {
        public override List<Type> PassStates => throw new NotImplementedException();

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }

        public override void OnStateTick(float tick)
        {
        }
    }
}
