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
                new GPPlayerData
                {
                    seat = 1,
                    hero = 100001,
                    position = new GPVector3(0, 0, 0),
                    euler = new GPVector3(0, 0, 0),
                    scale = new GPVector3(1000, 1000, 1000),
                },
            };
            
            stage = new Stage().Initialize(data);

            world = AddComp<World>().Initialize(1);
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

        protected override void OnSnapshot()
        {
            stage.Snapshot();
        }

        protected override void OnRestore()
        {
            stage.Restore();
        }

        private void OnFixedTick(FixedTickEvent e)
        {
            if (null == stage) return;
            if (StageState.Ticking != stage.state) return;

            var joystickdire = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                joystickdire += Vector2.up;
            }

            if (Input.GetKey(KeyCode.S))
            {
                joystickdire += Vector2.down;
            }

            if (Input.GetKey(KeyCode.A))
            {
                joystickdire += Vector2.left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                joystickdire += Vector2.right;
            }
            
            // 根据摄像机方向计算世界坐标系中的方向
            if (Vector2.zero != joystickdire && null != world.eyes.camera)
            {
                var cameraTransform = world.eyes.camera.transform;

                // 获取摄像机的前向和右向向量
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;

                // 忽略 Y 轴分量，保持平面运动
                forward.y = 0;
                right.y = 0;

                forward.Normalize();
                right.Normalize();

                // 将 joystickdire 转换为世界方向
                Vector3 worldDirection = joystickdire.x * right + joystickdire.y * forward;
                joystickdire = new Vector2(worldDirection.x, worldDirection.z);
            }
            

            input.joystickdire = joystickdire;
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