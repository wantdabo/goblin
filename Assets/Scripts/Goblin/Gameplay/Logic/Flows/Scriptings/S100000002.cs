using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings
{
    public class S100000002 : Scripting
    {
        public override uint id => FLOW_DEFINE.S100000002;
        
        protected override void OnScript()
        {
            Instruct(0, 5000, new BulletMotionData
            {
                motion = FLOW_BULLET_DEFINE.MOTION_STRAIGHT
            });
        }
    }
}