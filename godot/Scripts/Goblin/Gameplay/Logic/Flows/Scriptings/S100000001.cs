using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Kowtow.Math;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings;

public class S100000001 : Scripting
{
    public override uint id => FLOW_DEFINE.S100000001;

    protected override void OnScript()
    {
        for (int i = 18; i <= 360; i += 18)
        {
            Instruct(0, 40, new CreateMagicData
            {
                origin = FLOW_MAGIC_DEFINE.BORN_ORIGIN_OWNER,
                offset = new IntVector3(0, 0, 0),
                euler = FLOW_MAGIC_DEFINE.BORN_EULER_OWNER,
                angle = i * 1000,
                scale = 1000,
                pipelines = new List<uint> { FLOW_DEFINE.S100000002 }
            });
        }
    }
}