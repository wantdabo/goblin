using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 状态机状态
    /// </summary>
    public class StateMachineState : State
    {
        public override StateType type => StateType.StateMachine;
        /// <summary>
        /// 当前状态
        /// </summary>
        public byte current { get; set; }
        /// <summary>
        /// 上一个状态
        /// </summary>
        public byte last { get; set; }
        /// <summary>
        /// 持续帧数
        /// </summary>
        public uint frames { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public uint elapsed { get; set; }
        
        protected override void OnReset()
        {
            current = 0;
            last = 0;
            frames = 0;
            elapsed = 0;
        }
    }
}