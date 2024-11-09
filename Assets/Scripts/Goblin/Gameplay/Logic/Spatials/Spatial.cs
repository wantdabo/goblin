using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using TrueSync;

namespace Goblin.Gameplay.Logic.Spatials
{
    /// <summary>
    /// 空间平移变化事件
    /// </summary>
    public struct SpatialPositionChangedEvent : IEvent
    {
        /// <summary>
        /// 平移
        /// </summary>
        public TSVector position { get; set; }
    }

    /// <summary>
    /// 空间旋转变化事件
    /// </summary>
    public struct SpatialRotationChangedEvent : IEvent
    {
        /// <summary>
        /// 旋转
        /// </summary>
        public TSQuaternion rotation { get; set; }
    }

    /// <summary>
    /// 空间缩放事件
    /// </summary>
    public struct SpatialScaleChangedEvent : IEvent
    {
        /// <summary>
        /// 缩放
        /// </summary>
        public TSVector scale { get; set; }
    }

    /// <summary>
    /// 空间
    /// </summary>
    public class Spatial : Behavior<Translator>
    {
        private TSVector mposition { get; set; }
        /// <summary>
        /// 平移
        /// </summary>
        public TSVector position
        {
            get
            {
                return mposition;
            }
            set
            {
                mposition = new TSVector(value.x, value.y, 0);
                actor.eventor.Tell(new SpatialPositionChangedEvent { position = mposition });
            }
        }

        private TSQuaternion mrotation { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public TSQuaternion rotation
        {
            get
            {
                return mrotation;
            }
            set
            {
                mrotation = value;
                mrotation = TSQuaternion.Euler(0, mrotation.eulerAngles.y, 0);
                actor.eventor.Tell(new SpatialRotationChangedEvent { rotation = mrotation });
            }
        }

        private TSVector mscale { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
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

        /// <summary>
        /// 欧拉角
        /// </summary>
        public TSVector eulerAngles
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
