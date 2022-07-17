using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Common
{
    /// <summary>
    /// Client-Tick-Comp
    /// </summary>
    public abstract class CTickComp : Comp
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            if (this is IUpdate) Engine.CTEngine.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.CTEngine.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.CTEngine.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            if (this is IUpdate) Engine.CTEngine.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.CTEngine.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.CTEngine.RmvFixedUpdate(this as IFixedUpdate);

            base.OnDestroy();
        }
    }
}
