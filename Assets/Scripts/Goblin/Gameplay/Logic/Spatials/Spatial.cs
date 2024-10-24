using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using TrueSync;

namespace Goblin.Gameplay.Logic.Spatials
{
    public struct SpatialPositionChangedEvent : IEvent
    {
        public TSVector position { get; set; }
    }

    public struct SpatialRotationChangedEvent : IEvent
    {
        public TSQuaternion rotation { get; set; }
    }

    public struct SpatialScaleChangedEvent : IEvent
    {
        public TSVector scale { get; set; }
    }

    public class Spatial : Behavior<Translator>
    {
        private TSVector mposition { get; set; }
        public TSVector position
        {
            get
            {
                return mposition;
            }
            set
            {
                mposition = value;
                actor.eventor.Tell(new SpatialPositionChangedEvent { position = mposition });
            }
        }

        private TSQuaternion mrotation { get; set; }
        public TSQuaternion rotation
        {
            get
            {
                return mrotation;
            }
            set
            {
                mrotation = value;
                actor.eventor.Tell(new SpatialRotationChangedEvent { rotation = mrotation });
            }
        }
        private TSVector mscale { get; set; }
        public TSVector scale
        {
            get
            {
                return mscale;
            }
            set
            {
                mscale = value;
                actor.eventor.Tell(new SpatialScaleChangedEvent { scale = mscale });
            }
        }
        public TSVector eulerAngle
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = TSQuaternion.Euler(value.x, value.y, value.z);
            }
        }
    }
}
