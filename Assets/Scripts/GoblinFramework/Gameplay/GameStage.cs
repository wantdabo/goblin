using System.Collections.Generic;
using GoblinFramework.Common;
using GoblinFramework.Common.Events;
using GoblinFramework.Gameplay;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Events;
using UnityEngine;

namespace GoblinFramework.Gameplay
{
    public abstract class GameStage : Actor
    {
        private GameStatus mState = GameStatus.End;

        /// <summary>
        /// 关卡状态
        /// </summary>
        public GameStatus state
        {
            get { return mState; }
            private set
            {
                mState = value;
                eventor.Tell(new GameStatusEvent() { state = mState });
            }
        }

        /// <summary>
        /// 关卡配置
        /// </summary>
        protected GameStageConf rawStageConf;

        public GameConfig config = null;
        public Ticker ticker;

        private List<Actor> actors = new List<Actor>();
        private Dictionary<uint, Actor> actorDict = new Dictionary<uint, Actor>();

        protected override void OnCreate()
        {
            base.OnCreate();
            ticker = AddComp<Ticker>();
            ticker.eventor = ticker.AddComp<Eventor>();
            ticker.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ticker = null;
            config = null;
        }

        public void Tick(float tick)
        {
            Gaming(tick);
            ticker.Tick(tick);
        }

        /// <summary>
        /// 解析配置，构造关卡
        /// </summary>
        /// <param name="conf">关卡配置</param>
        public void Analyze(GameStageConf conf)
        {
            rawStageConf = conf;
            state = GameStatus.Analyze;
            OnAnalyze(conf);
        }

        /// <summary>
        /// 解析配置，构造关卡回调
        /// </summary>
        /// <param name="conf">关卡配置</param>
        protected abstract void OnAnalyze(GameStageConf conf);

        /// <summary>
        /// 开始/继续游戏
        /// </summary>
        public void Play()
        {
            state = GameStatus.Gaming;
            OnPlay();
        }

        /// <summary>
        /// 开始/继续游戏回调
        /// </summary>
        protected abstract void OnPlay();

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            state = GameStatus.Pause;
            OnPause();
        }

        /// <summary>
        /// 暂停游戏回调
        /// </summary>
        protected abstract void OnPause();

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="tick">帧率 (ms)</param>
        public void Gaming(float tick)
        {
            if (GameStatus.Gaming != state) return;

            OnGaming(tick);
        }

        /// <summary>
        /// 游戏中/Gameplay 循环回调
        /// </summary>
        /// <param name="tick">流逝的时间 (ms)</param>
        protected abstract void OnGaming(float tick);

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void End()
        {
            state = GameStatus.End;
            OnEnd();
        }

        /// <summary>
        /// 结束游戏回调
        /// </summary>
        protected abstract void OnEnd();

        public T GetActor<T>(uint id) where T : Actor
        {
            var actor = GetActor(id);

            if (null != actor) return actor as T;

            return null;
        }

        public Actor GetActor(uint id)
        {
            if (actorDict.TryGetValue(id, out var actor)) return actor;

            return null;
        }

        private uint actorIncrementId = 0;

        public T AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>();
            actor.id = ++actorIncrementId;
            actor.stage = this;
            actor.eventor = actor.AddComp<Eventor>();
            actor.eventor.Create();
            actors.Add(actor);
            actorDict.Add(actor.id, actor);
            eventor.Tell(new AddActorEvent() { actor = actor.id });

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
            eventor.Tell(new RmvActorEvent() { actor = actor.id });
        }
    }

    /// <summary>
    /// 玩法关卡基类
    /// </summary>
    /// <typeparam name="S">关卡本身</typeparam>
    /// <typeparam name="T">玩法配置</typeparam>
    public abstract class GameStage<S, T> : GameStage where S : GameStage<S, T>, new() where T : GameStageConf
    {
        protected T stageConf
        {
            get { return rawStageConf as T; }
            private set { rawStageConf = value; }
        }

        protected override void OnAnalyze(GameStageConf conf)
        {
            OnAnalyze(conf as T);
        }

        protected abstract void OnAnalyze(T stageConf);

        public static S CreateGameStage(GameConfig config)
        {
            var stage = new S();
            stage.parent = stage;
            stage.stage = stage;
            stage.eventor = stage.AddComp<Eventor>();
            stage.eventor.Create();
            stage.config = config;
            stage.Create();

            return stage;
        }
    }

    public abstract class GameStageConf
    {
    }

    /// <summary>
    /// 关卡状态
    /// </summary>
    public enum GameStatus
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
}