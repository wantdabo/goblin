using Goblin.Core;
using Goblin.Gameplay.Logic.Translations.Common;
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

        public void OnRILSync(uint id, uint frame, IRIL ril)
        {
            Actor actor = default;
            switch (ril.id)
            {
                case IRIL.LIVE_BORN:
                    stage.AddActor(id).Create();
                    break;
                case IRIL.LIVE_DEAD:
                    stage.GetActor(id).Destroy();
                    break;
            }

            actor = stage.GetActor(id);
            actor.eventor.Tell(new RILResolveEvent { frame = frame, ril = ril });
            Debug.Log($"RILSync: {actor}, {id}, {frame}, {ril}");
        }
    }
}
