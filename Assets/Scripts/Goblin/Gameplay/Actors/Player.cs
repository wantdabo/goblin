using Goblin.Gameplay.Common;
using Goblin.Gameplay.Core;
using Goblin.Gameplay.Inputs;
using Goblin.Gameplay.Render;
using Goblin.Gameplay.Render.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Gameplay.Actors
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
            AddBehavior<ModelRender>().Create();
            AddBehavior<SimpleAnimation>().Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
