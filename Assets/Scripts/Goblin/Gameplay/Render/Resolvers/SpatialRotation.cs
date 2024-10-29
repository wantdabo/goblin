using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 空间旋转解释器
    /// </summary>
    public class SpatialRotation : Resolver<RIL_SPATIAL_ROTATION>
    {
        private Node node { get; set; }
        
        private Quaternion lastRotation { get; set; }
        
        private Quaternion targetRotation { get; set; }
        
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

        protected override void OnAwake(uint frame, RIL_SPATIAL_ROTATION ril)
        {
            node = actor.EnsureBehavior<Node>();
            // 初始时将 lastRotation 设置为当前旋转
            lastRotation = node.go.transform.rotation;
        }

        protected override void OnResolve(uint frame, RIL_SPATIAL_ROTATION ril)
        {
            // TODO 后续需要统一插值算法
            // 保存当前的旋转
            lastRotation = node.go.transform.rotation;
            // 更新目标旋转
            targetRotation = ril.rotation.ToQuaternion();
            // 重置插值时间
            interpElapsed = 0f;
        }

        private void OnTick(TickEvent e)
        {
            // 增加插值时间
            interpElapsed += Time.deltaTime;
            // 使用 Slerp 进行球形插值，平滑过渡到新的旋转
            node.go.transform.rotation = Quaternion.Slerp(lastRotation, targetRotation, Mathf.Clamp01(interpElapsed / GameDef.LOGIC_TICK.AsFloat()));
        }
    }
}
