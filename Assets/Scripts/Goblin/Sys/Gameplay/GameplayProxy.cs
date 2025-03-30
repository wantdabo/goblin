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
        public Director director { get; private set; }

        public void Setup<T>() where T : Director, new()
        {
            if (null != director) return;
            director = AddComp<T>();
            director.Create();
            
            director.CreateGame();
            director.StartGame();
        }
    }
}
