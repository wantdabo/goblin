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
    /// Client-Comp，客户端组件
    /// </summary>
    public class CComp : Comp<RGEngine>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IUpdate) engine.ticker.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) engine.ticker.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) engine.ticker.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IUpdate) engine.ticker.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) engine.ticker.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) engine.ticker.RmvFixedUpdate(this as IFixedUpdate);
        }
    }
}
