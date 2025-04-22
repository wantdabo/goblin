using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 状态机状态
    /// </summary>
    public struct StateMachine : IState
    {
        public StateType type => StateType.StateMachine;
        public uint frame { get; set; }
        public ulong actor { get; set; }
    }
}