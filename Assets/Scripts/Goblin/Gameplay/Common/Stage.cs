using Goblin.Common;
using Goblin.Common.Network;
using Goblin.Gameplay.Render.Common;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TrueSync;

namespace Goblin.Gameplay.Common
{
    #region Events
    /// <summary>
    /// 添加 Actor/实体事件
    /// </summary>
    public struct AddActorEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor;
    }

    /// <summary>
    /// 移除 Actor/实体事件
    /// </summary>
    public struct RmvActorEvent : IEvent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public Actor actor;
    }

    #endregion

    /// <summary>
    /// Stage/关卡
    /// </summary>
    public class Stage : Actor
    {
        /// <summary>
        /// Actor 自增 ID
        /// </summary>
        private uint incrementId = 0;

        public NetNode net { get; private set; }

        /// <summary>
        /// 座位
        /// </summary>
        public uint seat { get; private set; }

        /// <summary>
        /// 最大帧
        /// </summary>
        public uint mframe { get; private set; }

        /// <summary>
        /// 帧
        /// </summary>
        public uint frame { get; private set; }

        /// <summary>
        /// 确定性，Ticker/时间驱动器
        /// </summary>
        public FPTicker ticker { get; private set; }
        /// <summary>
        /// 确定性，随机器
        /// </summary>
        public FPRandom random { get; private set; }

        /// <summary>
        /// Actor 列表
        /// </summary>
        public List<Actor> actors { get; private set; } = new();

        /// <summary>
        /// 玩家列表
        /// </summary>
        public Player[] players { get; private set; }

        /// <summary>
        /// 帧缓存
        /// </summary>
        private Queue<FrameInfo> frames = new();

        /// <summary>
        /// Actor 字典
        /// </summary>
        private Dictionary<uint, Actor> actorDict = new();

        /// <summary>
        /// 对局信息
        /// </summary>
        private S2C_GameInfoMsg conf;

        protected override void OnCreate()
        {
            base.OnCreate();
            ticker = AddComp<FPTicker>();
            ticker.Create();

            random = AddComp<FPRandom>();
            random.Initial(19491001);
            random.Create();

            net = AddComp<NetNode>();
            net.Create();

            net.Recv<G2C_StartStageMsg>(OnG2CStartStage);
            net.Recv<G2C_LogicTickMsg>(OnG2CLogicTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = actors.Count - 1; i >= 0; i--) actors[i].Destroy();
            net.UnRecv<G2C_StartStageMsg>(OnG2CStartStage);
            net.UnRecv<G2C_LogicTickMsg>(OnG2CLogicTick);
            net.Disconnect();
        }

        /// <summary>
        /// 解析配置，构造关卡
        /// </summary>
        /// <param name="conf">关卡配置</param>
        public void Analyze(S2C_GameInfoMsg conf)
        {
            this.conf = conf;
            net.Connect(conf.host, conf.port, 1);
            engine.ticker.Timing((t) =>
            {
                net.Send(new C2G_StartStageMsg { id = conf.id, seat = conf.seat, pid = conf.pid, password = conf.password });
            }, 0.4f, 1);
        }

        public void SetInput(int x)
        {
            net.Send(new C2G_SetInputMsg { inputInfo = new SeatInputInfo { seat = seat, moveX = x } });
        }

        /// <summary>
        /// 游戏中/Gameplay 循环
        /// </summary>
        /// <param name="frameInfo">帧信息</param>
        public void Gaming(FrameInfo frameInfo)
        {
            ticker.Tick();
            frame = frameInfo.frame;
            if (null == frameInfo.seatInputInfos) return;
            foreach (var input in frameInfo.seatInputInfos)
            {
                var gamepad = players[(int)input.seat].GetBehavior<Gamepad>();
                gamepad.SetInput(InputType.Joystick, new InputInfo { press = 0 != input.moveX, dire = new TSVector2(input.moveX > 0 ? 1 : -1, 0) });
            }
        }

        /// <summary>
        /// 获得 Actor（根据泛型转型）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="id">id</param>
        /// <returns>Actor</returns>
        public T GetActor<T>(uint id) where T : Actor
        {
            var actor = GetActor(id);

            if (null != actor) return actor as T;

            return null;
        }

        /// <summary>
        /// 获得 Actor
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Actor</returns>
        public Actor GetActor(uint id)
        {
            if (actorDict.TryGetValue(id, out var actor)) return actor;

            return null;
        }

        /// <summary>
        /// 添加 Actor
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>Actor</returns>
        public T AddActor<T>() where T : Actor, new()
        {
            var actor = AddComp<T>();
            actor.id = ++incrementId;
            actor.stage = this;
            actors.Add(actor);
            actorDict.Add(actor.id, actor);
            eventor.Tell(new AddActorEvent { actor = actor });

            return actor;
        }

        /// <summary>
        /// 根据 ID 移除 Actor
        /// </summary>
        /// <param name="id">ID</param>
        public void RmvActor(uint id)
        {
            var actor = GetActor(id);
            if (null == actor) return;

            RmvActor(actor);
        }

        /// <summary>
        /// 根据 Actor 实例移除 Actor
        /// </summary>
        /// <param name="actor">Actor 实例</param>
        public void RmvActor(Actor actor)
        {
            if (false == actors.Contains(actor)) return;

            actors.Remove(actor);
            actorDict.Remove(actor.id);
            eventor.Tell(new RmvActorEvent { actor = actor });
        }

        /// <summary>
        /// 对局初始化就绪
        /// </summary>
        private bool started = false;
        private void OnG2CStartStage(G2C_StartStageMsg msg)
        {
            mframe = msg.mframe;
            seat = msg.seat;
            players = new Player[msg.seatInfos.Length];
            for (int i = 0; i < msg.seatInfos.Length; i++)
            {
                players[i] = AddActor<Player>();
                players[i].Create();
                players[i].eventor.Tell(new LoadModelEvent { res = "Goblin" });
            }

            foreach (var frameInfo in msg.frameInfos) Gaming(frameInfo);
            started = true;
        }

        private void OnG2CLogicTick(G2C_LogicTickMsg msg)
        {
            frames.Enqueue(msg.frameInfo);

            if (false == started) return;

            while (frames.TryDequeue(out var frameInfo)) 
            {
                Gaming(frameInfo);
            }
        }
    }
}