using Goblin.Core;
using Goblin.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Goblin.Common.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class Machine : Comp
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public State current { get; private set; }

        /// <summary>
        /// 状态列表
        /// </summary>
        public List<State> states { get; private set; } = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>状态</returns>
        public T GetState<T>() where T : State
        {
            foreach (var s in states) if (typeof(T) == s.GetType()) return s as T;

            return null;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <exception cref="Exception">不能添加重复的状态</exception>
        public void SetState<T>() where T : State, new()
        {
            foreach (var s in states)
            {
                if (typeof(T) == s.GetType()) throw new Exception($"can'count set same state -> {typeof(T)}");
            }

            var state = AddComp<T>();
            state.machine = this;
            states.Add(state);
            state.Create();
        }

        /// <summary>
        /// 改变当前状态
        /// </summary>
        /// <param name="state">状态</param>
        public void ChangeState(State state)
        {
            if (state == current) return;
            current?.OnExit();

            current = state;
            if (null == current) return;
            current.OnEnter();
        }

        /// <summary>
        /// 状态进入条件检测
        /// </summary>
        private void StateDetect()
        {
            if (null != current && null == current.aisles) return;

            var nextState = current;
            foreach (var state in states)
            {
                if (state == current) continue;
                if (null != current && null == current.aisles) continue;
                if (null != current && false == current.aisles.Contains(state.GetType())) continue;
                if (false == state.OnCheck()) continue;
                nextState = state;
            }

            ChangeState(nextState);
        }

        private void OnTick(TickEvent e)
        {
            StateDetect();
            current?.OnTick(e.frame, e.tick);
        }
    }
}