using Goblin.Core;
using System;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Common.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class Machine : Comp
    {
        /// <summary>
        /// 状态机
        /// </summary>
        public ParallelMachine sm;

        /// <summary>
        /// 有限状态机层级
        /// </summary>
        public int layer;

        /// <summary>
        /// 当前状态
        /// </summary>
        public State current { get; private set; }

        /// <summary>
        /// 状态列表
        /// </summary>
        public List<State> allState { get => states; }

        /// <summary>
        /// 状态列表
        /// </summary>
        private List<State> states = new();

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
            sm.actor.eventor.Tell(new StateChangedEvent { state = current, layer = layer });
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

        public void OnFPTick(uint frame, FP tick)
        {
            StateDetect();
            current?.OnFPTick(frame, tick);
        }
    }
}