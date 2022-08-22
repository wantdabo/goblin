using BEPUutilities;
using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.Client.Gameplay.Resolves;
using GoblinFramework.Core;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Behaviors;
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

        public void FixedUpdate(float tick)
        {
            if (null == PGEngine) return;

            var actor = Theater.GetActor(1);
            Theater.CameraFollow.FollowActor = actor;

            Input joystick = new Input();
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
                joystick.dire += Vector2.Up;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
                joystick.dire += Vector2.Down;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
                joystick.dire += Vector2.Left;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
                joystick.dire += Vector2.Right;
            joystick.dire.Normalize();
            joystick.press = joystick.dire != Vector2.Zero;

            Input ba = new Input();
            ba.press = UnityEngine.Input.GetKey(UnityEngine.KeyCode.J);
            ba.dire = joystick.dire;

            Input bb = new Input();
            bb.press = UnityEngine.Input.GetKey(UnityEngine.KeyCode.K);
            bb.dire = joystick.dire;

            Input bc = new Input();
            bc.press = UnityEngine.Input.GetKey(UnityEngine.KeyCode.Space);
            bc.dire = joystick.dire;

            PGEngine.SetInput(1, InputType.Joystick, joystick);
            PGEngine.SetInput(1, InputType.BA, ba);
            PGEngine.SetInput(1, InputType.BB, bb);
            PGEngine.SetInput(1, InputType.BC, bc);

            PGEngine.TickEngine.PLoop();
        }
    }
}
