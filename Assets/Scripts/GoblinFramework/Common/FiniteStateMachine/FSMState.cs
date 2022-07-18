using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.FiniteStateMachine
{
    public abstract class FSMState : Goblin
    {
        protected override void OnCreate()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected virtual void OnEnter() { }

        protected virtual void OnLeave() { }
    }
}
