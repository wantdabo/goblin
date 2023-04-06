using System.Collections.Generic;
using GoblinFramework.Common;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Common;

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

    public abstract class GameStage : PComp
    {
        private StageState mState = StageState.End;

        /// <summary>
        /// 关卡状态
        /// </summary>
        public StageState state
        {
            get { return mState; }
            private set { mState = value; }
        }
        
        /// <summary>
        /// 关卡配置
        /// </summary>
        protected GameStageConf rawStageConf;
        
        public Ticker ticker = null;
        public GameConfig config = null;

        private List<Actor> actors = new List<Actor>();
        private Dictionary<uint, Actor> actorDict = new Dictionary<uint, Actor>();

        protected override void OnCreate()
        {
            base.OnCreate();
            ticker = AddComp<Ticker>();
            ticker.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ticker = null;
            config = null;
        }

        public Actor GetActor(uint id)
        {
            if (actorDict.TryGetValue(id, out var actor)) return actor;

            return null;
        }

        private uint actorIncrementId = 0;
        public Actor AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>();
            actor.id = ++actorIncrementId;
            actor.stage = this;
            actors.Add(actor);
            actorDict.Add(actor.id, actor);

            return actor;
        }

        public void RmvActor(uint id)
        {
            var actor = GetActor(id);
            if (null == actor) return;
            
            RmvActor(actor);
        }

        public void RmvActor(Actor actor)
        {
            actors.Remove(actor);
            actorDict.Remove(actor.id);
        }
        
        public void OnTick(float tick)
        {
            Gaming(tick);
            ticker.PLoop(tick);
        }
        
        /// <summary>
        /// 解析配置，构造关卡
        /// </summary>
        /// <param name="stageConf">关卡配置</param>
        public void Analyze(GameStageConf stageConf)
        {
            this.rawStageConf = stageConf;
            state = StageState.Analyze;
            OnAnalyze(stageConf);
        }

        /// <summary>
        /// 开始/继续游戏
        /// </summary>
        public void Play()
        {
            state = StageState.Gaming;
            OnPlay();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            state = StageState.Pause;
            OnPause();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void End()
        {
            state = StageState.End;
            OnEnd();
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="tick">帧率 (ms)</param>
        public void Gaming(float tick)
        {
            if (StageState.Gaming != state) return;

            OnGaming(tick);
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
            get { return rawStageConf as T; }
            private set { rawStageConf = value; }
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