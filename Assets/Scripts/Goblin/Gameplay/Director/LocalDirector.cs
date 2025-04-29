using Goblin.Common;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 本地单机, 本地导演
    /// </summary>
    public class LocalDirector : GameDirector
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
        /// 时间缩放 (本地游戏独有)
        /// </summary>
        public float timescale {
            get
            {
                return stage.GetBehaviorInfo<StageInfo>(stage.sa).timescale.AsFloat();
            }
            set
            {
                stage.GetBehaviorInfo<StageInfo>(stage.sa).timescale = ((int)(value * Config.Float2Int)) * stage.cfg.Int2FP;
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
        }

        protected override void OnDestroyGame()
        {
            // 销毁逻辑层
            stage.Dispose();
            // 取消监听 RIL 渲染状态
            stage.onril -= OnRIL;
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

        protected override void OnFixedTick(FixedTickEvent e)
        {
            base.OnFixedTick(e);
            if (null == stage) return;
            if (StageState.Ticking != stage.state) return;
            
            var joystick = world.input.GetInput(InputType.Joystick);
            stage.SetInput(world.selfseat, InputType.Joystick, joystick.press, joystick.dire);
            // 第二个单位, 镜像输入
            stage.SetInput(2, InputType.Joystick, joystick.press, new GPVector2(-joystick.dire.x, -joystick.dire.y));
            
            stage.Step();
        }
    }
}