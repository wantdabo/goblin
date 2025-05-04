using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;
using Queen.Protocols;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 锁步/帧同步导演
    /// </summary>
    public class LockstepDirector : GameDirector
    {
        /// <summary>
        /// 是否渲染 (驱动 World)
        /// </summary>
        public override bool rendering 
        {
            get
            {
                return null != stage && StageState.Ticking == stage.state;
            }
        }
        
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private Stage stage { get; set; }
        
        protected override void OnCreateGame()
        {
            // 初始化逻辑层
            stage = new Stage().Initialize(data.sdata);
            // 监听 RIL 渲染状态
            stage.onril += OnRIL;
            engine.net.Recv<S2CGameFrameMsg>(OnS2CGameFrame);
        }

        protected override void OnDestroyGame()
        {
            // 销毁逻辑层
            stage.Dispose();
            // 取消监听 RIL 渲染状态
            stage.onril -= OnRIL;
            engine.net.UnRecv<S2CGameFrameMsg>(OnS2CGameFrame);
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

        protected override void OnTick(TickEvent e)
        {
            base.OnTick(e);
            var joystick = world.input.GetInput(InputType.Joystick);
            engine.net.Send(new C2SPlayerInputMsg
            {
                id = data.id,
                seat = data.seat,
                inputs = new []{new PlayerInputData
                {
                    seat = data.seat,
                    type = (int)InputType.Joystick,
                    press = joystick.press,
                    dire = new Vector2
                    {
                        x = joystick.dire.x,
                        y = joystick.dire.y,
                    },
                }}
            });
        }

        /// <summary>
        /// 处理 RIL 渲染状态
        /// </summary>
        /// <param name="rilstate">RIL 渲染状态</param>
        private void OnRIL(RILState rilstate)
        {
            // 发送 RIL 渲染状态
            world.eventor.Tell(new RILEvent
            {
                rilstate = rilstate 
            });
        }

        private void OnS2CGameFrame(S2CGameFrameMsg msg)
        {
            // 此处因为网络，存在低概率，可能导致不同步，请勿用在正式项目。
            // 如果需要使用，请在此处添加锁步逻辑。
            // 对齐最新 frame，否则不予通过
            if (null != msg.frames)
            {
                // UnityEngine.Debug.Log($"OnS2CGameFrame -----> Histroy Length {msg.frames.Length}");
                foreach (var frame in msg.frames)
                {
                    SetFrameInput(frame.inputs);
                }
            }

            if (null == msg.frame) return;
            // UnityEngine.Debug.Log($"OnS2CGameFrame Current Frame -----> {msg.frame.frame}");
            if (1 != msg.frame.frame - stage.frame) return;

            // UnityEngine.Debug.Log($"OnS2CGameFrame SetFrameInput -----> {msg.frame.frame}");
            SetFrameInput(msg.frame.inputs);
        }

        private void SetFrameInput(PlayerInputData[] inputs)
        {
            foreach (var input in inputs)
            {
                stage.SetInput(input.seat, (InputType)input.type, input.press, new GPVector2
                {
                    x = input.dire.x,
                    y = input.dire.y,
                });
            }
            stage.Step();
        }
    }
}