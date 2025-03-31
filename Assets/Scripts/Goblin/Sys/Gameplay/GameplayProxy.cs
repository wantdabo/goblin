using Goblin.Common;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Kowtow;
using Kowtow.Math;
using UnityEngine;
using StateMachine = Goblin.Gameplay.Logic.Behaviors.StateMachine;
using Ticker = Goblin.Gameplay.Logic.Behaviors.Ticker;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 战斗 Proxy
    /// </summary>
    public class GameplayProxy : Proxy<GameplayModel>
    {
        private Director director { get; set; }

        public void Load<T>() where T : Director, new()
        {
            if (null != director)
            {
                director.StopGame();
                director.Destroy();
            }

            director = AddComp<T>();
            director.Create();
            
            director.CreateGame();
        }
        
        public void UnLoad()
        {
            if (null == director) return;
            
            director.StopGame();
            director.Destroy();
            director = null;
        }

        public void Start()
        {
            director.StartGame();
        }

        public void Pause()
        {
            director.PauseGame();
        }
        
        public void Resume()
        {
            director.ResumeGame();
        }
        
        public void Stop()
        {
            director.StopGame();
        }
    }
}
