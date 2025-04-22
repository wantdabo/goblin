using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 空间状态
    /// </summary>
    public struct SpatialState : IState
    {
        public StateType type => StateType.Spatial;
        public uint frame { get; set; }
        public ulong actor { get; set; }
    }
}