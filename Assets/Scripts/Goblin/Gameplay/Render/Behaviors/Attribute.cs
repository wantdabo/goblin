using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Attribute : Behavior
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public uint hp { get; set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public float movespeed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }
    }
}
