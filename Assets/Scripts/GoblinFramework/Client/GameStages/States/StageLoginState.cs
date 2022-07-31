using GoblinFramework.Client.UI.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class StageLoginState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GamePlayingState) };

        private LoginView loginView;
        protected override void OnEnter()
        {
            base.OnEnter();
            loginView = Engine.GameUI.OpenView<UI.Login.LoginView>();
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            Engine.GameUI.CloseView(loginView);
        }
    }
}
