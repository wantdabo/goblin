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
    public class GamePlayingState : GameStageState, IFixedUpdate
    {
        public override List<Type> PassStates => new List<Type> { typeof(StageLoginState) };

        private PGEngine PGEngine = null;
        private Theater Theater = null;
        protected override void OnEnter()
        {
            base.OnEnter();
            // TODO 临时代码，记得删除
            Theater = AddComp<Theater>();
            PGEngine = GameEngine<PGEngine>.CreateGameEngine();
            PGEngine.RegisterClientTheater(Theater);
            PGEngine.BeginGame();

            Engine.GameUI.OpenView<UI.Gameplay.GameplayView>();
        }

        int frame = 0;
        public void FixedUpdate(float tick)
        {
            if (null == PGEngine) return;

            frame += 1;
            PGEngine.TickEngine.PLoop(frame);
        }
    }
}
