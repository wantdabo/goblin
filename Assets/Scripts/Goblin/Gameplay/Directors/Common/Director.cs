using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Directors.Common
{
    public abstract class Director : Comp
    {
        public abstract bool rendering { get; }
        protected GPData data { get; private set; }
        public World world { get; protected set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        public void CreateGame(GPData data)
        {
            this.data = data;
            world = AddComp<World>().Initialize(data.seat);
            world.Create();
            OnCreateGame();
        }

        public void DestroyGame()
        {
            OnDestroyGame();
            world.Destroy();
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
        
        private void OnTick(TickEvent e)
        {
            if (false == rendering) return;

            world.ticker.Tick(e.tick);
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