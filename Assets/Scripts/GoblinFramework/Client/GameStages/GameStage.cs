using GoblinFramework.Core.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStage : FSMachineLockstep<CGEngine, GameStage, GameStageState>
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
