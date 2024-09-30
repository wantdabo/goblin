using Goblin.Gameplay.Logic.Translation;
using Goblin.Gameplay.Logic.Translation.Common;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Transform
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
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_SPATIAL_POSITION(position));
            }

            if (false == behavior.rotation.Equals(rotation))
            {
                rotation = behavior.rotation;
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_SPATIAL_ROTATION(rotation));
            }

            if (false == behavior.scale.Equals(scale))
            {
                scale = behavior.scale;
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_SPATIAL_SCALE(scale));
            }
        }
    }
}
