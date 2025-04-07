using Goblin.Common;
using Goblin.Common.FSM;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.GameplayDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

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

        private World world { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        protected override void OnCreateGame()
        {
            GameplayData data = new GameplayData();
            data.seed = 19491001;
            data.players = new[]
            {
                new PlayerData { hero = 10010 },
                new PlayerData { hero = 10010 },
            };
            
            stage = new Stage();
            stage.Initialize(data);

            world = AddComp<World>();
            world.Create();

            input = AddComp<InputSystem>();
            input.Create();

            stage.onril += (id, ril) => world.eventor.Tell(new RILEvent { state = new ABStateInfo(id, ril) });
        }

        protected override void OnDestroyGame()
        {
            stage.Dispose();
            world.Destroy();
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

            stage.Step();
        }

        private void OnTick(TickEvent e)
        {
            if (null == stage) return;
            if (StageState.Ticking != stage.state) return;

            world.ticker.Tick(e.tick);
        }
    }
}