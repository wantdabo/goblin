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
            paramachine.SetState<PlayerAttack>();
            
            // TODO 后续要改成配置文件读取
            var launcher = GetBehavior<SkillLauncher>();
            launcher.Load(10001);
            launcher.Load(10002);
            launcher.Load(10003);
            launcher.Load(10004);
            launcher.Load(10011);
            launcher.Load(10012);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
