using Goblin.Core;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Directors.Common
{
    public abstract class Director : Comp
    {
        protected GPData data { get; private set; }
        public World world { get; protected set; }
        
        public void CreateGame(GPData data)
        {
            this.data = data;
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
        
        public void Snapshot()
        {
            OnSnapshot();
        }
        
        public void Restore()
        {
            OnRestore();
        }

        protected abstract void OnCreateGame();
        protected abstract void OnDestroyGame();
        protected abstract void OnStartGame();
        protected abstract void OnPauseGame();
        protected abstract void OnResumeGame();
        protected abstract void OnStopGame();
        protected abstract void OnSnapshot();
        protected abstract void OnRestore();
    }
}