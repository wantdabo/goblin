﻿using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core.FSMachine
{

    /// <summary>
    /// Finite-State-Machine-State，状态机，状态
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMState<E, MT, ST> : Comp<E> where E : GameEngine<E>, new() where MT : FSMachine<E, MT, ST>, new() where ST : FSMState<E, MT, ST>, new()
    {
        /// <summary>
        /// 状态机
        /// </summary>
        public MT Machine;

        public void Enter()
        {
            OnEnter();
        }

        public void Leave()
        {
            OnLeave();
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected abstract void OnEnter();
        /// <summary>
        /// 状态离开
        /// </summary>
        protected abstract void OnLeave();
        /// <summary>
        /// 状态驱动
        /// </summary>
        /// <param name="tick">变值步长</param>
        public abstract void OnStateTick(float tick);
    }
}
