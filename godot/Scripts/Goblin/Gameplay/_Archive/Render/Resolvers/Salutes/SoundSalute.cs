using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Salutes;

/// <summary>
/// 音效事件处理器
/// </summary>
public class SoundSalute : RILSalute<RIL_EVENT_SOUND>
{
    protected override void OnSalute(RIL_EVENT_SOUND e)
    {
        var agent = rilbucket.world.EnsureAgent<SoundAgent>(e.actor);
        agent.Play(e);
    }
}
