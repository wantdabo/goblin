using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings;

// Born 管线：t=0 强制切换到 IDLE 状态
public class S10000 : Scripting
{
    public override uint id => FLOW_DEFINE.S10000;

    protected override void OnScript()
    {
        Instruct(0, 0, new ChangeStateData
        {
            state = STATE_DEFINE.IDLE,
            force = true,
        });
    }
}
