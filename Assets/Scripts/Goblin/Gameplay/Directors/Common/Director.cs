using Goblin.Core;

namespace Goblin.Gameplay.Directors.Common
{
    public abstract class Director : Comp
    {
        public void CreateGame()
        {
            OnCreateGame();
        }

        public void DestroyGame()
        {
            OnDestroyGame();
        }

        public void StartGame()
        {
            OnStartGame();
        }

        public void PauseGame()
        {
            OnPauseGame();
        }

        public void ResumeGame()
        {
            OnResumeGame();
        }

        public void StopGame()
        {
            OnStopGame();
        }

        protected abstract void OnCreateGame();
        protected abstract void OnDestroyGame();
        protected abstract void OnStartGame();
        protected abstract void OnPauseGame();
        protected abstract void OnResumeGame();
        protected abstract void OnStopGame();
    }
}