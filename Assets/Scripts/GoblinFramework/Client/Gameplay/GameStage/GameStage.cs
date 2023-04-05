using GoblinFramework.Client.Common;

namespace GoblinFramework.Client.Gameplay
{
   /// <summary>
    /// 关卡状态
    /// </summary>
    public enum StageState
    {
        /// <summary>
        /// 解析中
        /// </summary>
        Analyze,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 结束
        /// </summary>
        End,
        /// <summary>
        /// 游戏中
        /// </summary>
        Gaming
    }

    public abstract class GameStage : CComp, IFixedUpdate
    {
        private StageState mStageState = StageState.End;
        /// <summary>
        /// 关卡状态
        /// </summary>
        public StageState stageState
        {
            get { return mStageState; }
            private set { mStageState = value; }
        }
        
        /// <summary>
        /// 关卡配置
        /// </summary>
        protected GameStageConf baseStageConf;

        /// <summary>
        /// 解析配置，构造关卡
        /// </summary>
        /// <param name="stageConf">关卡配置</param>
        public void Analyze(GameStageConf stageConf)
        {
            this.baseStageConf = stageConf;
            stageState = StageState.Analyze;
            OnAnalyze(stageConf);
        }

        /// <summary>
        /// 开始/继续游戏
        /// </summary>
        public void Play()
        {
            stageState = StageState.Gaming;
            OnPlay();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            stageState = StageState.Pause;
            OnPause();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void End()
        {
            stageState = StageState.End;
            OnEnd();
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="tick">帧率 (ms)</param>
        public void Gaming(float tick)
        {
            if (StageState.Gaming != stageState) return;

            OnGaming(tick);
        }

        public void FixedUpdate(float tick)
        {
            Gaming(tick);
        }

        /// <summary>
        /// 解析配置，构造关卡回调
        /// </summary>
        /// <param name="stageConf">关卡配置</param>
        public abstract void OnAnalyze(GameStageConf stageConf);
        
        /// <summary>
        /// 开始/继续游戏回调
        /// </summary>
        public abstract void OnPlay();
        
        /// <summary>
        /// 暂停游戏回调
        /// </summary>
        public abstract void OnPause();
        
        /// <summary>
        /// 结束游戏回调
        /// </summary>
        public abstract void OnEnd();
        
        /// <summary>
        /// 游戏中/Gameplay 循环回调
        /// </summary>
        /// <param name="tick">流逝的时间 (ms)</param>
        public abstract void OnGaming(float tick);
    }

    /// <summary>
    /// 玩法关卡基类
    /// </summary>
    /// <typeparam name="T">玩法配置</typeparam>
    public abstract class GameStage<T> : GameStage where T : GameStageConf
    {
        protected T stageConf
        {
            get { return baseStageConf as T; }
            private set { baseStageConf = value; }
        }

        public override void OnAnalyze(GameStageConf stageConf)
        {
            OnAnalyze(stageConf as T);
        }
        
        public abstract void OnAnalyze(T stageConf);
    }

    public abstract class GameStageConf
    {

    }
}