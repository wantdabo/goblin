using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows.Scriptings;

// 死亡管线：t=0 切换到 DEATH 状态，随后销毁 Actor
public class S10001 : Scripting
{
    public override uint id => FLOW_DEFINE.S10001;

    protected override void OnScript()
    {
        Instruct(0, 0, new ChangeStateData
        {
            state = STATE_DEFINE.DEATH,
            force = true,
        });

        Instruct(0, 0, new RmvActorData());
    }
}
