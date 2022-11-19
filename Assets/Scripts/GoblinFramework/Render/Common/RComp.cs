using GoblinFramework.Render;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Render.Common
{
    /// <summary>
    /// Render-Comp，渲染层组件
    /// </summary>
    public class RComp : Comp<CGEngine>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IUpdate) Engine.TickEngine.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.TickEngine.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.TickEngine.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IUpdate) Engine.TickEngine.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.TickEngine.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.TickEngine.RmvFixedUpdate(this as IFixedUpdate);
        }
    }
}
