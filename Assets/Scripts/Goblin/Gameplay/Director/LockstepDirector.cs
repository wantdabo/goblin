using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
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

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Recv<S2CGameFrameMsg>(OnS2CGameFrame);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnRecv<S2CGameFrameMsg>(OnS2CGameFrame);
        }

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

        private void OnS2CGameFrame(S2CGameFrameMsg msg)
        {
        }
    }
}