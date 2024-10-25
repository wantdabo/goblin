using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Lives;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
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
        public bool gaming { get; private set; }
        public bool dancing { get; set; }
        public InputSystem input { get; private set; }
        public Stage stage { get; private set; }
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

        public void Start()
        {
            Time.fixedDeltaTime = 1f / GameDef.LOGIC_FRAME;
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
            player.eventor.Tell<LiveBornEvent>();

            var player2 = lstage.AddActor<Player>();
            player2.Create();
            player2.eventor.Tell<LiveBornEvent>();
            
            engine.gameui.Open<GameplayView>();
            
            Resume();
        }

        public void Resume()
        {
            gaming = true;
        }

        public void Pause()
        {
            gaming = false;
        }

        public void End() { }

        private void OnTick(TickEvent e)
        {
            if (false == gaming) return;
            stage.Tick(e.tick);
        }
        
        private void OnFixedTick(FixedTickEvent e)
        {
            if (false == gaming) return;
            input.Input(1, lstage);
            lstage.Tick();
        }
    }
}
