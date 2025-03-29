using Goblin.Common;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Core;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Kowtow;
using Kowtow.Math;
using UnityEngine;
using Ticker = Goblin.Gameplay.Logic.Behaviors.Ticker;

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
        public float gamespeed { get; set; } = 1f;
        /// <summary>
        /// 伤害跳字开关
        /// </summary>
        public bool dancing { get; set; }
        /// <summary>
        /// 输入系统
        /// </summary>
        public InputSystem input { get; private set; }
        /// <summary>
        /// 逻辑场景
        /// </summary>
        private Stage stage { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            input = AddComp<InputSystem>();
            input.Create();
            
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }

        public void CreateGame()
        {
            stage = new Stage();
            stage.Initialize(19491001, null);

            var actor = stage.AddActor();
            actor.AddBehavior<Attribute>();
            actor.AddBehavior<Ticker>();
            actor.AddBehavior<Spatial>();
            
        }

        public void StartGame()
        {
            stage.Start();
        }

        public void PauseGame()
        {
            stage.Pause();
        }

        public void ResumeGame()
        {
            stage.Resume();
        }

        public void StopGame()
        {
            stage.Stop();
        }

        private void OnFixedTick(FixedTickEvent e)
        {
            if (null == stage) return;
            stage.Tick();
        }
    }
}
