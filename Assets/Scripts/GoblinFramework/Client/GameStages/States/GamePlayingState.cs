using GoblinFramework.Client.Common;
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
        protected override void OnEnter()
        {
            base.OnEnter();
            PGEngine = GameEngine<PGEngine>.CreateGameEngine();
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
