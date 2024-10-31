using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// Buff 信息解释器
    /// </summary>
    public class BuffInfo : Resolver<RIL_BUFF_INFO>
    {
        private BuffBucket bucket { get; set; }

        protected override void OnAwake(uint frame, RIL_BUFF_INFO ril)
        {
            bucket = actor.EnsureBehavior<BuffBucket>();
        }
        
        protected override void OnResolve(uint frame, RIL_BUFF_INFO ril)
        {
            bucket.Set(ril.buffid, (ril.state, ril.layer, ril.maxlayer));
        }
    }
}
