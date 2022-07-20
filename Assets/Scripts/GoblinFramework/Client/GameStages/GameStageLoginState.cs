using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GameStageLoginState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GameStageGamingState) };

        protected override void OnEnter()
        {
            base.OnEnter();
            Engine.GameUI.OpenView<UI.Login.LoginView>();
        }
    }
}
