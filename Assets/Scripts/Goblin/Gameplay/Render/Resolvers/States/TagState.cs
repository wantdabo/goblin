using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 标签状态
    /// </summary>
    public struct TagState : IState
    {
        public StateType type => StateType.Tag;
        public uint frame { get; set; }
        public ulong actor { get; set; }
    }
}