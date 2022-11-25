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
    /// Finite-State-Machine，有限状态机
    /// </summary>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="MIT">状态机 BeaviorInfo 类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMachine<MT, MIT, ST> : Behavior<MIT>, ILoop where MIT : BehaviorInfo, new() where MT : FSMachine<MT, MIT, ST>, new() where ST : FSMState<MT, MIT, ST>, new()
    {
        protected ST state;
        public ST State { get { return state; } private set { state = value; } }

        protected Type entranceState;
        public Type EntranceState { get { return entranceState; } private set { entranceState = value; } }

        protected Dictionary<Type, ST> stateDict = new Dictionary<Type, ST>();
        protected List<ST> stateList = new List<ST>();

        public T GetState<T>() where T : ST
        {
            return GetState(typeof(T)) as T;
        }

        public ST GetState(Type type)
        {
            stateDict.TryGetValue(type, out ST targetState);

            return targetState;
        }

        protected void SetState<T>() where T : ST, new()
        {
            var state = AddComp<T>();
            state.Behavior = this as MT;
            stateDict.Add(typeof(T), state);
            stateList.Add(state);
        }

        public void SetEntrance<T>() where T : ST
        {
            SetEntrance(typeof(T));
        }

        public void SetEntrance(Type type)
        {
            if (null != EntranceState) throw new Exception("entrance only setting once.");
            EntranceState = type;
            Entrance();
        }

        public void Entrance()
        {
            if (null == EntranceState) throw new Exception("entrancestate is null, plz do this at setting entrancestate later.");
            EnterState(GetState(EntranceState));
        }

        private void EnterState(ST targetState)
        {
            if (targetState == State) return;

            if (null != State)
            {
                if (null == State.PassStates) return;
                if (false == State.PassStates.Contains(targetState.GetType())) return;

                State.Leave();
            }

            targetState.Enter();
        }

        public virtual void OnEnter(ST state)
        {
            State = state;
        }

        public virtual void OnLeave(ST state)
        {
            State = null;
        }

        private void StateDetect()
        {
            var detect = false;
            foreach (var state in stateList)
            {
                if (state.OnDetectEnter())
                {
                    detect = true;
                    EnterState(state);
                }
            }

            if (detect) return;
            Entrance();
        }

        public virtual void PLoop(int frame, FP detailTime)
        {
            if (State != null && State.OnDetectLeave()) State.Leave();
            StateDetect();
            State?.OnStateTick(frame, detailTime);
        }
    }
}
