using GoblinFramework.Client.Comps;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client
{
    public abstract class ClientComp : Comp
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            if (this is IUpdate) Engine.EngineTick.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.EngineTick.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.EngineTick.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            if (this is IUpdate) Engine.EngineTick.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.EngineTick.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.EngineTick.RmvFixedUpdate(this as IFixedUpdate);

            base.OnDestroy();
        }
    }
}
