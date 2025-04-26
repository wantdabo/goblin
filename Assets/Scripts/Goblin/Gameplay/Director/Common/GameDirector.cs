using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Director.Common
{
    /// <summary>
    /// 导演, 负责指挥游戏的运行, 包括创建游戏, 销毁游戏, 开始游戏, 暂停游戏, 恢复游戏, 停止游戏等操作
    /// 驱动渲染层 World, 渲染状态的接收以及传入 World
    /// </summary>
    public abstract class GameDirector : Comp
    {
        /// <summary>
        /// 是否渲染 (驱动 World)
        /// </summary>
        public abstract bool rendering { get; }
        /// <summary>
        /// 游戏数据
        /// </summary>
        protected GPData data { get; private set; }
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; protected set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }

        /// <summary>
        /// 创建游戏
        /// </summary>
        /// <param name="data">游戏数据</param>
        public void CreateGame(GPData data)
        {
            this.data = data;
            world = AddComp<World>().Initialize(data.seat);
            world.Create();
            OnCreateGame();
        }

        /// <summary>
        /// 销毁游戏
        /// </summary>
        public void DestroyGame()
        {
            OnDestroyGame();
            world.Destroy();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            OnStartGame();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            OnPauseGame();
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            OnResumeGame();
        }

        /// <summary>
        /// 停止游戏
        /// </summary>
        public void StopGame()
        {
            OnStopGame();
        }
        
        /// <summary>
        /// 快照
        /// </summary>
        public void Snapshot()
        {
            OnSnapshot();
        }
        
        /// <summary>
        /// 恢复
        /// </summary>
        public void Restore()
        {
            OnRestore();
        }
        
        private void OnTick(TickEvent e)
        {
            if (false == rendering) return;

            world.ticker.Tick(e.tick);
        }
        
        private void OnFixedTick(FixedTickEvent e)
        {
            OnStep();
        }

        /// <summary>
        /// 创建游戏
        /// </summary>
        protected abstract void OnCreateGame();
        /// <summary>
        /// 销毁游戏
        /// </summary>
        protected abstract void OnDestroyGame();
        /// <summary>
        /// 开始游戏
        /// </summary>
        protected abstract void OnStartGame();
        /// <summary>
        /// 暂停游戏
        /// </summary>
        protected abstract void OnPauseGame();
        /// <summary>
        /// 恢复游戏
        /// </summary>
        protected abstract void OnResumeGame();
        /// <summary>
        /// 停止游戏
        /// </summary>
        protected abstract void OnStopGame();
        /// <summary>
        /// 快照
        /// </summary>
        protected abstract void OnSnapshot();
        /// <summary>
        /// 恢复
        /// </summary>
        protected abstract void OnRestore();
        /// <summary>
        /// 驱动
        /// </summary>
        protected abstract void OnStep();
    }
}