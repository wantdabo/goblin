using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 空间解释器
    /// </summary>
    public class Spatial : Resolver<RIL_SPATIAL>
    {
        private Node node { get; set; }
        
        private Vector3 position { get; set; }
        
        private Vector3 lastPosition { get; set; }
        
        private float interpElapsed { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        protected override void OnAwake(uint frame, RIL_SPATIAL ril)
        {
            node = actor.EnsureBehavior<Node>();
            lastPosition = node.go.transform.position;
        }

        protected override void OnResolve(uint frame, RIL_SPATIAL ril)
        {
            // TODO 后续需要统一插值算法
            // 记录前一帧的位置
            lastPosition = position;
            // 更新当前位置
            position = ril.position.ToVector3();
            // 重置插值时间
            interpElapsed = 0f;
        }

        private void OnTick(TickEvent e)
        {
            interpElapsed += Time.deltaTime;
            // 使用 Lerp 进行线性插值，平滑过渡到新的位置
            node.go.transform.position = Vector3.Lerp(lastPosition, position, Mathf.Clamp01(interpElapsed / GAME_DEFINE.LOGIC_TICK.AsFloat()));
        }
    }
}
