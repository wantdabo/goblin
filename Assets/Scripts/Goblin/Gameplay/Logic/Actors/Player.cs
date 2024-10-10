using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Attributes.Surfaces;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.States.Player;
using Goblin.Gameplay.Logic.Spatials;

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
            
            var paramachine = GetBehavior<ParallelMachine>();
            paramachine.SetState<Idle>();
            paramachine.SetState<Run>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
