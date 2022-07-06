using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.Tick
{
    /// <summary>
    /// 对应 Unity Update
    /// </summary>
    internal interface IUpdate { public void Update(float tick); }

    /// <summary>
    /// 对应 Unity LateUpdate
    /// </summary>
    internal interface ILateUpdate { public void LateUpdate(float tick); }

    /// <summary>
    /// 对应 Unity FixedUpdate
    /// </summary>
    internal interface IFixedUpdate { public void FixedUpdate(float tick); }

    internal class EngineTick : Goblin, IUpdate, ILateUpdate, IFixedUpdate
    {
        private List<IUpdate> updates = null;
        private List<ILateUpdate> lateUpdates = null;
        private List<IFixedUpdate> fixedUpdates = null;

        public override void OnCreate()
        {
            updates = new List<IUpdate>();
            lateUpdates = new List<ILateUpdate>();
            fixedUpdates = new List<IFixedUpdate>();
        }

        public override void OnDestroy()
        {
            updates.Clear();
            updates = null;

            lateUpdates.Clear();
            lateUpdates = null;

            fixedUpdates.Clear();
            fixedUpdates = null;
        }

        public void Update(float tick)
        {
            foreach (var update in updates) { update.Update(tick); }
        }

        public void LateUpdate(float tick)
        {
            foreach (var lateUpdate in lateUpdates) { lateUpdate.LateUpdate(tick); }
        }

        public void FixedUpdate(float tick)
        {
            foreach (var fixedUpdate in fixedUpdates) { fixedUpdate.FixedUpdate(tick); }
        }
    }
}
