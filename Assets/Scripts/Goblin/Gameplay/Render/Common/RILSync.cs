using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// RIL/ 渲染指令同步
    /// </summary>
    public class RILSync : Comp
    {
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.eventor.Listen<RILSyncEvent>(OnRILSync);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.eventor.UnListen<RILSyncEvent>(OnRILSync);
        }

        private void OnRILSync(RILSyncEvent e)
        {
            Actor actor = default;
            actor = stage.GetActor(e.id);
            actor.eventor.Tell(new RILResolveEvent { frame = e.frame, ril = e.ril });
            // Debug.Log($"RILSync -> {actor}, {e.id}, {e.frame}, {e.ril}");
        }
    }
}
