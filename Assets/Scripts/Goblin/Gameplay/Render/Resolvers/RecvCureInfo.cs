using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Sys.Gameplay;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 受到治疗解释器
    /// </summary>
    public class RecvCureInfo : Resolver<RIL_RECV_CURE_INFO>
    {
        private Node node { get; set; }
        private Attribute attribute { get; set; }
        
        protected override void OnAwake(uint frame, RIL_RECV_CURE_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
            attribute = actor.EnsureBehavior<Attribute>();
        }
        
        protected override void OnResolve(uint frame, RIL_RECV_CURE_INFO ril)
        {
            engine.proxy.gameplay.eventor.Tell(new CureDanceEvent
            {
                position = node.go.transform.position,
                cure = ril.cure,
                from = actor.id,
                to = actor.id
            });
        }
    }
}
