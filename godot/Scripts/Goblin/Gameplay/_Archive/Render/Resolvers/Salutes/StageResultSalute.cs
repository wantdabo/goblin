using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Sys.Common;

namespace Goblin.Gameplay.Render.Resolvers.Salutes;

/// <summary>
/// 关卡结果事件处理器
/// </summary>
public class StageResultSalute : RILSalute<RIL_EVENT_STAGE_RESULT>
{
    protected override void OnSalute(RIL_EVENT_STAGE_RESULT e)
    {
        engine.proxy.gameplay.eventor.Tell(new StageResultEvent { win = e.win });
    }
}
