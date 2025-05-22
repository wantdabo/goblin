using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Goblin.Common;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 本地单机, 本地导演
    /// </summary>
    public class LocalDirector : GameplayDirector
    {
        /// <summary>
        /// 是否渲染 (驱动 World)
        /// </summary>
        public override bool rendering
        {
            get { return null != stage && StageState.Ticking == stage.state; }
        }

        /// <summary>
        /// 时间缩放 (本地游戏独有)
        /// </summary>
        public float timescale
        {
            get { return stage.GetBehaviorInfo<StageInfo>(stage.sa).timescale.AsFloat(); }
            set { stage.GetBehaviorInfo<StageInfo>(stage.sa).timescale = ((int)(value * Config.Float2Int)) * stage.cfg.int2fp; }
        }
        
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private Stage stage { get; set; }
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private readonly object @lock = new();
        /// <summary>
        /// RIL 队列
        /// </summary>
        private readonly Queue<IRIL> rilqueue = new();
        /// <summary>
        /// RIL_DIFF 队列
        /// </summary>
        private readonly Queue<IRIL_DIFF> diffqueue = new();

        protected override void OnCreateGame()
        {
            // 初始化逻辑层
            stage = new Stage().Initialize(data.sdata);
            // 监听 RIL 渲染状态
            stage.onril += OnRIL;
            // 监听 RIL_DIFF 渲染状态
            stage.ondiff += OnDiff;
        }

        protected override void OnDestroyGame()
        {
            // 销毁逻辑层
            stage.Dispose();
            // 取消监听 RIL 渲染状态
            stage.onril -= OnRIL;
            // 取消监听 RIL_DIFF 渲染状态
            stage.ondiff -= OnDiff;
        }

        protected override void OnStartGame() => stage.Start();
        protected override void OnPauseGame() => stage.Pause();
        protected override void OnResumeGame() => stage.Resume();
        protected override void OnStopGame() => stage.Stop();

        protected override void OnSnapshot()
        {
            world.Snapshot();
            stage.Snapshot();
        }

        protected override void OnRestore()
        {
            world.rilbucket.LossAllRIL();
            stage.Restore();
            world.Restore();
        }

        protected override void OnTick()
        {
            lock (@lock)
            {
                // 发送 RIL 渲染状态
                while (rilqueue.Count > 0) world.rilbucket.SetRIL(rilqueue.Dequeue());
            }
        }

        protected override void OnStep()
        {
            if (null == stage || StageState.Ticking != stage.state) return;

            var joystick = world.input.GetInput(INPUT_DEFINE.JOYSTICK);
            var ba = world.input.GetInput(INPUT_DEFINE.BA);
            stage.SetInput(world.selfseat, INPUT_DEFINE.JOYSTICK, joystick.press, joystick.dire);
            stage.SetInput(world.selfseat, INPUT_DEFINE.BA, ba.press, ba.dire);
            stage.Step();
        }

        /// <summary>
        /// 处理 RIL 渲染状态
        /// </summary>
        /// <param name="ril">RIL 渲染状态</param>
        private void OnRIL(IRIL ril)
        {
            lock (@lock)
            {
                rilqueue.Enqueue(ril);
            }
        }

        /// <summary>
        /// 处理 RIL_DIFF 渲染状态
        /// </summary>
        /// <param name="diff">差异状态</param>
        private void OnDiff(IRIL_DIFF diff)
        {
            lock (@lock)
            {
                diffqueue.Enqueue(diff);
            }
        }
    }
}