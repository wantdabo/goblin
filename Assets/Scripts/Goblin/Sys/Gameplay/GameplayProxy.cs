﻿using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Kowtow;
using Kowtow.Math;
using Queen.Protocols;
using UnityEngine;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 战斗 Proxy
    /// </summary>
    public class GameplayProxy : Proxy<GameplayModel>
    {
        public GameplayDirector director { get; private set; }
        /// <summary>
        /// 绘制物理
        /// </summary>
        public bool physdraw { get; set; } = false;
        /// <summary>
        /// 跳字
        /// </summary>
        public bool dancing { get; set; } = false;

        /// <summary>
        /// 加载战斗
        /// </summary>
        /// <param name="data">游戏数据</param>
        /// <param name="multithread">是否多线程</param>
        /// <typeparam name="T">战斗驱动器</typeparam>
        public void Load<T>(BuildData data, bool multithread = false) where T : GameplayDirector, new()
        {
            Time.fixedDeltaTime = GAME_DEFINE.LOGIC_TICK.AsFloat();
            if (null != director)
            {
                director.StopGame();
                director.Destroy();
            }

            director = AddComp<T>();
            director.Create();
            
            director.CreateGame(data, multithread);
        }
        
        /// <summary>
        /// 卸载战斗
        /// </summary>
        public void UnLoad()
        {
            if (null == director) return;
            
            director.StopGame();
            director.Destroy();
            director = null;
        }
    }
}
