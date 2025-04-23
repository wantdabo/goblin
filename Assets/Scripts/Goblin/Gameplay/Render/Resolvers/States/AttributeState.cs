using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 属性状态
    /// </summary>
    public class AttributeState : State
    {
        public override StateType type => StateType.Attribute;

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
        public uint movespeed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }
        
        protected override void OnReset()
        {
            hp = 0;
            maxhp = 0;
            movespeed = 0;
            attack = 0;
        }
    }
}