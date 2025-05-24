using System;
using System.Diagnostics;
using System.Threading;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Director.Common
{
    /// <summary>
    /// 导演, 负责指挥游戏的运行, 包括创建游戏, 销毁游戏, 开始游戏, 暂停游戏, 恢复游戏, 停止游戏等操作
    /// 驱动渲染层 World, 渲染状态的接收以及传入 World
    /// </summary>
    public abstract class GameplayDirector : Comp
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
        /// <summary>
        /// 是否多线程
        /// </summary>
        public bool multithread { get; private set; }
        /// <summary>
        /// 子线程
        /// </summary>
        private Thread thread { get; set; }
        /// <summary>
        /// 逻辑 Step 耗时
        /// </summary>
        public int stepms { get; private set; }

        /// <summary>
        /// 创建游戏
        /// </summary>
        /// <param name="data">游戏数据</param>
        public void CreateGame(GPData data, bool multithread = false)
        {
            this.data = data;
            this.multithread = multithread;
            world = AddComp<World>().Initialize(data.seat);
            world.Create();
            OnCreateGame();
            
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            if (false == multithread)
            {
                engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
                return;
            }
            thread = new Thread(() =>
            {
                int logicms = (int)GAME_DEFINE.LOGIC_TICK_MS;
                while (true)
                {
                    var ms = DateTime.Now.Millisecond;
                    OnStep();
                    stepms = Math.Clamp(DateTime.Now.Millisecond - ms, 0, int.MaxValue);
                    
                    if (stepms <= logicms)
                    {
                        Thread.Sleep(logicms - stepms);
                    }
                }
            });
            thread.Start();
        }

        /// <summary>
        /// 销毁游戏
        /// </summary>
        public void DestroyGame()
        {
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            if (false == multithread)
            {
                engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
            }
            else
            {
                try
                {
                    thread.Abort();
                    thread = null;
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            
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
        
        protected void OnTick(TickEvent e)
        {
            if (false == rendering) return;
            OnTick();
            world.ticker.Tick(e.tick);
        }
        
        protected void OnFixedTick(FixedTickEvent e)
        {
            stepms = (int)(e.tick * 1000);
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
        /// 渲染层驱动 (单线程)
        /// </summary>
        protected abstract void OnTick();
        /// <summary>
        /// 逻辑层驱动 (根据配置决定单线程还是多线程)
        /// </summary>
        protected abstract void OnStep();
    }
}