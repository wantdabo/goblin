namespace Goblin.Gameplay.Logic.Prefabs.Datas
{
    /// <summary>
    /// 属性数据结构
    /// </summary>
    public struct Attribute
    {
        /// <summary>
        /// 当前生命值
        /// </summary>
        public uint hp { get; set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public uint moveseed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }
    }
}