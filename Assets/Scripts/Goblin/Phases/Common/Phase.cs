﻿using Goblin.Common.FSM;
using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Phases.Common
{
    /// <summary>
    /// 游戏阶段管理
    /// </summary>
    public class Phase : Comp
    {
        /// <summary>
        /// 状态机
        /// </summary>
        private Machine machine;

        protected override void OnCreate()
        {
            base.OnCreate();
            machine = AddComp<Machine>();
            machine.Create();
            machine.SetState<ResPhase>();
            machine.SetState<HotfixPhase>();
            machine.SetState<LoginPhase>();
            machine.SetState<GamingPhase>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 获取阶段
        /// </summary>
        /// <typeparam name="T">阶段类型</typeparam>
        /// <returns>阶段</returns>
        public T GetPhase<T>() where T : State 
        {
            return machine.GetState<T>();
        }
    }
}