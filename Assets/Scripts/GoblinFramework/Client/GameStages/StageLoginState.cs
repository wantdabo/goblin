using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class StageLoginState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(GamingState) };

        protected override void OnEnter()
        {
            base.OnEnter();
            Engine.GameUI.OpenView<UI.Login.LoginView>();
        }
    }
}
