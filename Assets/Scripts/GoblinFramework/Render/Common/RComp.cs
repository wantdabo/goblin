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
    public class RComp : Comp<RGEngine>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IUpdate) Engine.Ticker.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.Ticker.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.Ticker.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IUpdate) Engine.Ticker.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.Ticker.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.Ticker.RmvFixedUpdate(this as IFixedUpdate);
        }
    }
}
