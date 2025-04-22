using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 场景状态
    /// </summary>
    public struct StageState : IState
    {
        public StateType type => StateType.Stage;
        public uint frame { get; set; }
        public ulong actor { get; set; }
    }
}