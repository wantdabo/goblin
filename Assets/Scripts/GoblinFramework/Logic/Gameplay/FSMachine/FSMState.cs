using GoblinFramework.Core;
using GoblinFramework.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay
{
    /// <summary>
    /// Finite-State-Machine，有限状态机，状态
    /// </summary>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMState<MT, ST> : Comp where MT : FSMachine<MT, ST>, new() where ST : FSMState<MT, ST>, new()
    {
        /// <summary>
        /// 定义可通行的状态类型列表，如果下一个装填类型不在此列表中，将不允通过
        /// </summary>
        public abstract List<Type> PassStates { get; }

        /// <summary>
        /// 进入状态，消耗了多少逻辑帧
        /// </summary>
        private int elapsedFrames = 0;
        /// <summary>
        /// 进入状态，消耗了多少逻辑帧
        /// </summary>
        public int ElapsedFrames { get { return elapsedFrames; } private set { elapsedFrames = value; } }

        /// <summary>
        /// 状态机
        /// </summary>
        public MT Behavior;

        public void Enter()
        {
            elapsedFrames = 0;
            Behavior.OnEnter(this as ST);
            OnEnter();
        }

        public void Leave()
        {
            OnLeave();
            Behavior.OnLeave(this as ST);
        }

        /// <summary>
        /// 状态检查，能否进入
        /// </summary>
        /// <returns>true 能进，false 不能进</returns>
        public abstract bool OnDetectEnter();

        /// <summary>
        /// 状态检查，是否要退出
        /// </summary>
        /// <returns>true 退出，false 不退出</returns>
        public abstract bool OnDetectLeave();

        /// <summary>
        /// 状态进入
        /// </summary>
        protected abstract void OnEnter();

        /// <summary>
        /// 状态离开
        /// </summary>
        protected abstract void OnLeave();

        /// <summary>
        /// 状态 Tick
        /// </summary>
        /// <param name="frame">帧数</param>
        public virtual void OnStateTick(int frame, FP detailTime) { elapsedFrames++; }
    }
}
