using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    /// <summary>
    /// 泛型游戏引擎组件
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    public class GameEngine<E> : Comp<E> where E : GameEngine<E>, new()
    {
        /// <summary>
        /// 创建一个游戏引擎
        /// </summary>
        /// <returns>游戏引擎组件</returns>
        public static E CreateGameEngine() 
        {
            E engine = new E();
            engine.parent = engine;
            engine.engine = engine;
            engine.Create();

            return engine;
        }
    }
}
