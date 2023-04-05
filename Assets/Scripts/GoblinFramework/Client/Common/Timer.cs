using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Common
{
    /// <summary>
    /// Goblin-Timer 渲染层计时器
    /// </summary>
    public class Timer : CComp, IUpdate
    {
        private bool mIsRunning = false;
        public bool isRunning { get { return mIsRunning; } private set { mIsRunning = value; } }

        private Action action;
        private float interval;
        private int count;
        private float elapse;

        public void Start(Action action, float interval, int count = 1)
        {
            Create();
            if (0 == count) throw new Exception("clock count can't be zero(0)");
            this.action = action;
            this.interval = interval;
            this.count = count;
            this.elapse = 0;

            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
            parent.RmvComp(this);
            Destroy();
        }

        public void Update(float tick)
        {
            if (false == isRunning) return;

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
