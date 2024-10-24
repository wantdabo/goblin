using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action.Common
{
    /// <summary>
    /// 三维向量数据
    /// </summary>
    [MessagePackObject(true)]
    public class Vector3Data
    {
        /// <summary>
        /// X 轴
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Y 轴
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// Z 轴
        /// </summary>
        public int z { get; set; }
        
        /// <summary>
        /// 三维向量数据
        /// </summary>
        /// <param name="x">X 轴</param>
        /// <param name="y">Y 轴</param>
        /// <param name="z">Z 轴</param>
        public Vector3Data(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
