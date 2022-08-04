using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.Core;
using GoblinFramework.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.GameStages
{
    public class GamePlayingState : GameStageState
    {
        public override List<Type> PassStates => new List<Type> { typeof(StageLoginState) };

        public Theater Theater = null;
        public PGEngine PGEngine = null;

        protected override void OnEnter()
        {
            base.OnEnter();
            Engine.GameUI.OpenView<UI.Gameplay.GameplayView>();

            Theater = AddComp<Theater>();
            PGEngine = GameEngine<PGEngine>.CreateGameEngine();
            PGEngine.RegisterRILRecv(Theater, (ril) => Theater.Resolve(ril));
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
    }
}
