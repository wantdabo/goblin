using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageGameResState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GameStageHotfixState), typeof(GameStageLoginState) };

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnStateTick(float tick)
        {
            if (true == Engine.GameRes.Ready) Engine.GameStage.EnterState<GameStageLoginState>();
        }
    }
}
