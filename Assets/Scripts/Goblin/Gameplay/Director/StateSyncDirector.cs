using Goblin.Gameplay.Director.Common;

namespace Goblin.Gameplay.Director
{
    /// <summary>
    /// 状态同步导演
    /// </summary>
    public class StateSyncDirector : GameDirector
    {
        /// <summary>
        /// 是否渲染 (驱动 World)
        /// </summary>
        public override bool rendering { get; }

        protected override void OnCreateGame()
        {
        }

        protected override void OnDestroyGame()
        {
        }

        protected override void OnStartGame()
        {
        }

        protected override void OnPauseGame()
        {
        }

        protected override void OnResumeGame()
        {
        }

        protected override void OnStopGame()
        {
        }

        protected override void OnSnapshot()
        {
        }

        protected override void OnRestore()
        {
        }

        protected override void OnStep()
        {
        }
    }
}