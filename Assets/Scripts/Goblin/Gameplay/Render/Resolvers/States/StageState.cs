using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 场景状态
    /// </summary>
    public class StageState : State
    {
        public override StateType type => StateType.Stage;
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// Actor 数量
        /// </summary>
        public uint actorcnt { get; set; }
        /// <summary>
        /// Behavior 数量
        /// </summary>
        public uint behaviorcnt { get; set; }
        /// <summary>
        /// BehaviorInfo 数量
        /// </summary>
        public uint behaviorinfocnt { get; set; }
        /// <summary>
        /// 是否有快照
        /// </summary>
        public bool hassnapshot { get; set; }
        /// <summary>
        /// 快照帧号
        /// </summary>
        public uint snapshotframe { get; set; }
        
        protected override void OnReset()
        {
            frame = 0;
            actorcnt = 0;
            behaviorcnt = 0;
            behaviorinfocnt = 0;
            hassnapshot = false;
            snapshotframe = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is StageState state)
            {
                return frame == state.frame &&
                       actorcnt == state.actorcnt && 
                       behaviorcnt == state.behaviorcnt &&
                       behaviorinfocnt == state.behaviorinfocnt && hassnapshot == state.hassnapshot &&
                       snapshotframe == state.snapshotframe;
            }
            
            return false;
        }
    }
}