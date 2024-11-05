using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Sys.Gameplay;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 受到伤害解释器
    /// </summary>
    public class RecvHurtInfo : Resolver<RIL_RECV_HURT_INFO>
    {
        private Node node { get; set; }
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_RECV_HURT_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
            attribute = actor.EnsureBehavior<Attribute>();
        }
        
        protected override void OnResolve(uint frame, RIL_RECV_HURT_INFO ril)
        {
            engine.sound.Load("onhit_0001").Play();
            engine.proxy.gameplay.eventor.Tell(new DamageDanceEvent
            {
                position = node.go.transform.position,
                crit = ril.crit,
                damage = ril.value,
                from = actor.id,
                to = actor.id
            });
        }
    }
}
