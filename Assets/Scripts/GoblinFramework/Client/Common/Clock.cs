using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Common
{
    /// <summary>
    /// Clock 客户端计时器
    /// </summary>
    public class Clock : Comp<CGEngine>, IUpdate
    {
        private bool isRunning = false;
        public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }

        private Action action;
        private float interval;
        private int count;
        private float elapse;

        public void Start(Action action, float interval, int count = 1)
        {
            if (0 == count) throw new Exception("clock count can't be zero(0)");
            this.action = action;
            this.interval = interval;
            this.count = count;
            this.elapse = 0;

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            parent.RmvComp(this);
        }

        public void Update(float tick)
        {
            if (false == IsRunning) return;

            elapse = elapse + tick;
            if (elapse > interval)
            {
                count--;
                elapse = elapse - interval;
                action.Invoke();
            }
            if (0 == count) Stop();
        }
    }
}
