using GoblinFramework.Client.Common;
using GoblinFramework.Client.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStage : FSMachineLockstep<GameStage, GameStageState>
    {
        protected override void OnCreate()
        {
            SetState<GameInitializeState>();
            SetState<GameHotfixState>();
            SetState<StageLoginState>();
            SetState<GamePlayingState>();

            base.OnCreate();
        }
    }
}
