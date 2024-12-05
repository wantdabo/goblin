using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Lives;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Logic.Skills;
using Goblin.Gameplay.Logic.Spatials;
using Goblin.Gameplay.Render.Core;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Kowtow;
using Kowtow.Math;
using UnityEngine;
using LStage = Goblin.Gameplay.Logic.Core.Stage;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 治疗跳字
    /// </summary>
    public struct CureDanceEvent : IEvent
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 position { get; set; }
        /// <summary>
        /// 治疗数值
        /// </summary>
        public uint cure { get; set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from { get; set; }
        /// <summary>
        /// 去向/ActorID
        /// </summary>
        public uint to { get; set; }
    }

    /// <summary>
    /// 伤害跳字
    /// </summary>
    public struct DamageDanceEvent : IEvent
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 position { get; set; }
        /// <summary>
        /// 暴击
        /// </summary>
        public bool crit { get; set; }
        /// <summary>
        /// 伤害数值
        /// </summary>
        public uint damage { get; set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from { get; set; }
        /// <summary>
        /// 去向/ActorID
        /// </summary>
        public uint to { get; set; }
    }

    /// <summary>
    /// 战斗 Proxy
    /// </summary>
    public class GameplayProxy : Proxy<GameplayModel>
    {
        /// <summary>
        /// 游戏中
        /// </summary>
        public bool gaming { get; private set; }
        /// <summary>
        /// 游戏速度
        /// </summary>
        public sbyte gamespeed { get; set; } = 1;
        /// <summary>
        /// 伤害跳字开关
        /// </summary>
        public bool dancing { get; set; }
        /// <summary>
        /// 输入系统
        /// </summary>
        public InputSystem input { get; private set; }
        /// <summary>
        /// 渲染场景
        /// </summary>
        public Stage stage { get; private set; }
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private LStage lstage { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
            input = AddComp<InputSystem>();
            input.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }
        
        /// <summary>
        /// 生成木桩
        /// </summary>
        private void GenEnemys()
        {
            for (int i = 2; i < 7; i++)
            {
                var enemy = lstage.AddActor<Player>();
                enemy.Create();
                var spatial = enemy.GetBehavior<Spatial>();
                spatial.position = new FPVector3(i, FP.One, FP.Zero);
                spatial.eulerAngles = new FPVector3(0, -90, 0);
                enemy.live.Born();
            }
            
            var enemy2 = lstage.AddActor<Player>();
            enemy2.Create();
            var spatial2 = enemy2.GetBehavior<Spatial>();
            spatial2.position = new FPVector3(19, FP.One, FP.Zero);
            spatial2.eulerAngles = new FPVector3(0, -90, 0);
            enemy2.live.Born();
        }
        
        /// <summary>
        /// 游戏开始
        /// </summary>
        public void Start()
        {
            Time.fixedDeltaTime = GAME_DEFINE.LOGIC_TICK.AsFloat();
            lstage = AddComp<LStage>();
            lstage.Create();

            stage = AddComp<Stage>();
            stage.Create();
            stage.foc.SetFollow(1);
            lstage.eventor.Listen<RILSyncEvent>((e) =>
            {
                stage.eventor.Tell(e);
            });
            lstage.eventor.Listen<PhysShapesEvent>((e) =>
            {
                stage.eventor.Tell(e);
            });

            var player = lstage.AddActor<Player>();
            player.Create();
            player.GetBehavior<Spatial>().position = new FPVector3(FP.Zero, FP.One, FP.Zero);
            player.live.Born();
            
            // GenEnemys();

            engine.gameui.Open<GameplayView>();

            Resume();
        }
        
        /// <summary>
        /// 游戏恢复
        /// </summary>
        public void Resume()
        {
            gaming = true;
        }

        /// <summary>
        /// 游戏暂停
        /// </summary>
        public void Pause()
        {
            gaming = false;
        }
        
        /// <summary>
        /// 游戏结束
        /// </summary>
        public void End() { }

        private void OnTick(TickEvent e)
        {
            if (false == gaming) return;
            for (int i = 0; i < gamespeed; i++) stage.Tick(e.tick);
        }

        private void OnFixedTick(FixedTickEvent e)
        {
            if (false == gaming) return;

            input.Input(1, lstage);
            for (int i = 0; i < gamespeed; i++) lstage.Tick();
        }
    }
}
