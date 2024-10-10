using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Attributes.Surfaces;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills;
using Goblin.Gameplay.Logic.States.Player;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Actors
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class Player : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<Surface>().Create();
            AddBehavior<Spatial>().Create();
            AddBehavior<Gamepad>().Create();
            AddBehavior<ParallelMachine>().Create();
            AddBehavior<SkillLauncher>().Create();
            
            var paramachine = GetBehavior<ParallelMachine>();
            paramachine.SetState<PlayerIdle>();
            paramachine.SetState<PlayerRun>();
            
            // 侧转
            GetBehavior<Spatial>().eulerAngle = TSVector.up * 90;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
