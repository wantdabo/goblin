using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.Core;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Behaviors;
using Numerics.Fixed;
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

        public override void OnStateTick(float tick)
        {
            base.OnStateTick(tick);

            Input input = new Input();
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
                input.dire += Fixed64Vector2.Up;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
                input.dire += Fixed64Vector2.Down;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
                input.dire += Fixed64Vector2.Left;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
                input.dire += Fixed64Vector2.Right;

            input.press = input.dire != Fixed64Vector2.Zero;

            PGEngine.SetInput(1, InputType.Joystick, input);
        }

        public void FixedUpdate(float tick)
        {
            PGEngine?.TickEngine.PLoop();
        }
    }
}
