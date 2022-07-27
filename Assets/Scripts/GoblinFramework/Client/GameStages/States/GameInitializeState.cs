using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.UI.GameInitialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameInitializeState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GameHotfixState), typeof(StageLoginState) };

        private GameInitializeView view = null;

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            view?.Close();
        }

        public override void OnStateTick(float tick)
        {
            if (true == Engine.GameRes.Ready && null == view) view = Engine.GameUI.OpenView<GameInitializeView>();

            if (view?.progress >= 1) Engine.GameStage.EnterState<StageLoginState>();
        }
}
}
