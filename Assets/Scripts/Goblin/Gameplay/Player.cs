using Goblin.Gameplay.Common;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.States.Common;
using Goblin.Gameplay.States.Player;
using GoblinFramework.Gameplay.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Gameplay
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class Player : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<Node>().Create();
            AddBehavior<Gamepad>().Create();
            AddBehavior<StateMachine>().Create();
            AddBehavior<ModelRender>().Create();
            AddBehavior<SimpleAnimation>().Create();

            var sm = GetBehavior<StateMachine>();
            sm.SetState<PlayerIdleState>();
            sm.SetState<PlayerRunningState>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
