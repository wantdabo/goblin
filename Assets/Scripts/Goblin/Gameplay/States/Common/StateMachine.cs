using Goblin.Common;
using Goblin.Gameplay.Common;
using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.States.Common
{
    /// <summary>
    /// 状态改变事件
    /// </summary>
    public struct SMStateChangedEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public FSMState state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 状态进入事件
    /// </summary>
    public struct SMStateEnterEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public FSMState state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 状态离开事件
    /// </summary>
    public struct SMStateExitEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public FSMState state;
        /// <summary>
        /// 层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 层次状态机
    /// </summary>
    public class StateMachine : Behavior
    {
        /// <summary>
        /// 有限状态机集合
        /// </summary>
        private FSMachine[] fsms = new FSMachine[2];
        /// <summary>
        /// 当前状态集合
        /// </summary>
        private FSMState[] states = new FSMState[2];
        /// <summary>
        /// 当前状态
        /// </summary>
        public FSMState curstate { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            actor.eventor.Listen<SMStateChangedEvent>(OnSMStateChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
            actor.eventor.UnListen<SMStateChangedEvent>(OnSMStateChanged);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        /// <returns>状态</returns>
        public T GetState<T>(int layer = 0) where T : FSMState
        {
            var fsm = fsms[layer];
            if (null == fsm) return null;

            return fsm.GetState<T>();
        }

        /// <summary>
        /// 设置状态机状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="layer">层级</param>
        public void SetState<T>(int layer = 0) where T : FSMState, new()
        {
            var fsm = fsms[layer];

            if (null == fsm)
            {
                fsm = AddComp<FSMachine>();
                fsm.sm = this;
                fsm.layer = layer;
                fsms[layer] = fsm;
                fsm.Create();
            }

            fsm.SetState<T>();
        }

        private void OnSMStateChanged(SMStateChangedEvent e)
        {
            curstate = e.state;
            states[e.layer] = e.state;
        }

        private void OnFPTick(FPTickEvent e)
        {
            for (int i = 0; i < fsms.Length; i++)
            {
                var fsm = fsms[i];
                if (null != fsm) fsm.OnFPTick(e.frame, e.tick);
            }
        }
    }
}
