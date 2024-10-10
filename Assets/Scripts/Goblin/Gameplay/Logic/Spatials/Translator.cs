using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Spatials
{
    public class Translator : Translator<Spatial>
    {
        private TSVector position { get; set; }
        private TSQuaternion rotation { get; set; }
        private TSVector scale { get; set; }

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
