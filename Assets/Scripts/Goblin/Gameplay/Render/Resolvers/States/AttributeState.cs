using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 属性状态
    /// </summary>
    public struct AttributeState : IState
    {
        public StateType type => StateType.Attribute;
        
        public uint frame { get; set; }
        public ulong actor { get; set; }
    }
}