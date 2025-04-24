using Goblin.Common;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Kowtow.Math;
using UnityEngine;

namespace Goblin.Gameplay.Directors
{
    public class LocalDirector : Director
    {
        public override bool rendering 
        {
            get
            {
                return null != stage && StageState.Ticking == stage.state;
            }
        }
        
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

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnCreateGame()
        {
            stage = new Stage().Initialize(data.sdata);
            stage.onril += OnRIL;
        }

        protected override void OnDestroyGame()
        {
            stage.Dispose();
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

        private void OnRIL(RILState rilstate)
        {
            world.eventor.Tell(new RILEvent
            {
                rilstate = rilstate 
            });
        }

        private void OnFixedTick(FixedTickEvent e)
        {
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