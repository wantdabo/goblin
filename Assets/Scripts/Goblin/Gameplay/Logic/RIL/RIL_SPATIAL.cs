using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 空间渲染指令
    /// </summary>
    public struct RIL_SPATIAL : IRIL
    {
        public ushort id => RIL_DEFINE.SPATIAL;
        /// <summary>
        /// 平移
        /// </summary>
        public FPVector3 position { get; private set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public FPVector3 euler { get; private set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public FPVector3 scale { get; private set; }

        /// <summary>
        /// 空间渲染指令
        /// </summary>
        /// <param name="position">平移</param>
        /// <param name="euler">旋转</param>
        /// <param name="scale">缩放</param>
        public RIL_SPATIAL(FPVector3 position, FPVector3 euler, FPVector3 scale)
        {
            this.position = position;
            this.euler = euler;
            this.scale = scale;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL _other)
            {
                return _other.position.Equals(position) && _other.euler.Equals(euler) && _other.scale.Equals(scale);
            }

            return false;
        }

        public override string ToString()
        {
            return $"RIL_SPATIAL: position={position}, euler={euler}, scale={scale}";
        }
    }
}
