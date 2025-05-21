using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 空间渲染指令
    /// </summary>
    public class RIL_SPATIAL : IRIL
    {
        public override ushort id => RIL_DEFINE.SPATIAL;
        /// <summary>
        /// 位置
        /// </summary>
        public FPVector3 position { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public FPVector3 euler { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public FPVector3 scale { get; set; }

        protected override void OnReady()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FPVector3.one;
        }

        protected override void OnReset()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FPVector3.one;
        }

        protected override void OnCopy(IRIL target)
        {
            if (target is not RIL_SPATIAL ril) return;
            ril.position = position;
            ril.euler = euler;
            ril.scale = scale;
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"RIL_SPATIAL: position={position}, euler={euler}, scale={scale}";
        }
    }
}
