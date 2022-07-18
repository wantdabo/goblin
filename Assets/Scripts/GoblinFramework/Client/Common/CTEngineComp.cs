using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Common
{
    /// <summary>
    /// 对应 Unity Update
    /// </summary>
    public interface IUpdate { public void Update(float tick); }

    /// <summary>
    /// 对应 Unity LateUpdate
    /// </summary>
    public interface ILateUpdate { public void LateUpdate(float tick); }

    /// <summary>
    /// 对应 Unity FixedUpdate
    /// </summary>
    public interface IFixedUpdate { public void FixedUpdate(float tick); }

    /// <summary>
    /// Client-Tick-Engine-Comp, 客户端 Tick 驱动组件
    /// </summary>
    public class CTEngineComp : Comp<CGEngineComp>
    {
        private List<IUpdate> updates = new List<IUpdate>();
        private List<ILateUpdate> lateUpdates = new List<ILateUpdate>();
        private List<IFixedUpdate> fixedUpdates = new List<IFixedUpdate>();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            updates.Clear();
            updates = null;

            lateUpdates.Clear();
            lateUpdates = null;

            fixedUpdates.Clear();
            fixedUpdates = null;
            base.OnDestroy();
        }

        public void AddUpdate(IUpdate update) { updates.Add(update); }
        public void RmvUpdate(IUpdate update) { updates.Remove(update); }
        public void AddLateUpdate(ILateUpdate lateUpdate) { lateUpdates.Add(lateUpdate); }
        public void RmvLateUpdate(ILateUpdate lateUpdate) { lateUpdates.Remove(lateUpdate); }
        public void AddFixedUpdate(IFixedUpdate fixedUpdate) { fixedUpdates.Add(fixedUpdate); }
        public void RmvFixedUpdate(IFixedUpdate fixedUpdate) { fixedUpdates.Remove(fixedUpdate); }

        public void Update(float tick)
        {
            if (0 > updates.Count) return;
            foreach (var update in updates) update.Update(tick);
        }

        public void LateUpdate(float tick)
        {
            if (0 > lateUpdates.Count) return;
            foreach (var lateUpdate in lateUpdates) lateUpdate.LateUpdate(tick);
        }

        public void FixedUpdate(float tick)
        {
            if (0 > fixedUpdates.Count) return;
            foreach (var fixedUpdate in fixedUpdates) fixedUpdate.FixedUpdate(tick);
        }
    }
}
