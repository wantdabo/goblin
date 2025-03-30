using Goblin.Common;
using Goblin.Common.FSM;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Core;
using UnityEngine;
using Ticker = Goblin.Gameplay.Logic.Behaviors.Ticker;

namespace Goblin.Gameplay.Directors.Local.Common
{
    public class LocalDirector : Director
    {
        /// <summary>
        /// 输入系统
        /// </summary>
        public InputSystem input { get; private set; }
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private Stage stage { get; set; }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            input = AddComp<InputSystem>();
            input.Create();
            
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnCreateGame()
        {
            stage = new Stage();
            stage.Initialize(19491001, null);

            var actor = stage.AddActor();
            actor.AddBehaviorInfo<AttributeInfo>();
            actor.AddBehaviorInfo<SpatialInfo>();
            actor.AddBehavior<Ticker>();
            actor.AddBehavior<Gamepad>();
            actor.AddBehavior<StateMachine>();
            actor.AddBehavior<Movement>();
            
            var attribute = actor.GetBehaviorInfo<AttributeInfo>();
            attribute.moveseed = 10;
        }

        protected override void OnStartGame()
        {
            stage.Start();
        }

        protected override void OnPauseGame()
        {
            stage.Pause();
        }

        protected override void OnResumeGame()
        {
            stage.Resume();
        }

        protected override void OnStopGame()
        {
            stage.Stop();
        }
        
        private void OnFixedTick(FixedTickEvent e)
        {
            if (null == stage) return;
            if (StageState.Ticking != stage.state) return;
            
            input.joystickdire = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W))
            {
                input.joystickdire += Vector2.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                input.joystickdire += Vector2.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                input.joystickdire += Vector2.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                input.joystickdire += Vector2.right;
            }

            input.Input(1, stage);

            var spatial = stage.GetBehaviorInfo<SpatialInfo>(1);
            Debug.Log($"Spatial.Position ---------> {spatial.position}");

            stage.Tick();
        }
    }
}