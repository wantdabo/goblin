using GoblinFramework.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageComp : FSMLockstepComp<CGEngineComp, GameStageComp, GameStageState>
    {
        protected override void OnCreate()
        {
            SetState<GameStageGameInitializeState>();
            SetState<GameStageHotfixState>();
            SetState<GameStageLoginState>();
            SetState<GameStageGamingState>();

            base.OnCreate();
        }
    }
}
