using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Kowtow.Math;
using System;

namespace Goblin.Gameplay.Logic.Spatials
{
    /// <summary>
    /// 空间翻译
    /// </summary>
    public class Translator : Translator<Spatial>
    {
        /// <summary>
        /// 平移
        /// </summary>
        private FPVector3 position { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        private FPQuaternion rotation { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        private FPVector3 scale { get; set; }

        protected override void OnRIL()
        {
            if (false == behavior.position.Equals(position))
            {
                position = behavior.position;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SPATIAL_POSITION(position));
            }

            if (false == behavior.rotation.Equals(rotation))
            {
                rotation = behavior.rotation;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SPATIAL_ROTATION(rotation));
            }

            if (false == behavior.scale.Equals(scale))
            {
                scale = behavior.scale;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SPATIAL_SCALE(scale));
            }
        }
    }
}
