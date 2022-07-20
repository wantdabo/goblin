using GoblinFramework.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageState : FSMLockstepState<CGEngineComp, GameStageComp, GameStageState>
    {
        public override List<Type> PassStates => throw new NotImplementedException();

        protected override void OnCreate()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }
    }
}
