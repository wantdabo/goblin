using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings
{
    public class S100000001 : Scripting
    {
        public override uint id => FLOW_DEFINE.S100000001;

        protected override void OnScript()
        {
            for (int i = 18; i <= 360; i += 18)
            {
                Instruct(0, 40, new CreateBulletData
                {
                    strength = 1000,
                    speed = 5000,
                    origin = FLOW_BULLET_DEFINE.BORN_ORIGIN_OWNER,
                    offset = new GPVector3(0, 0, 0),
                    euler = FLOW_BULLET_DEFINE.BORN_EULER_OWNER,
                    angle = i * 1000,
                    scale = 1000,
                    pipelines = new List<uint> { FLOW_DEFINE.S100000002 }
                });
            }
        }
    }
}