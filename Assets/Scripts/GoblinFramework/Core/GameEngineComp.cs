using GoblinFramework.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    /// <summary>
    /// 游戏引擎组件
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    public class GameEngineComp<E> : Comp<E> where E : GameEngineComp<E>, new()
    {
        /// <summary>
        /// 通用引擎组件
        /// </summary>
        public GGEngineComp G;

        /// <summary>
        /// 创建一个游戏引擎
        /// </summary>
        /// <returns>游戏引擎组件</returns>
        public static E CreateGameEngine() 
        {
            GGEngineComp gengine = new GGEngineComp();
            gengine.Engine = gengine;
            gengine.Create();

            E engine = new E();
            engine.Engine = engine;
            engine.G = gengine;
            engine.Create();

            return engine;
        }
    }
}
