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

            if (this is IUpdate) Engine.CETick.AddUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.CETick.AddLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.CETick.AddFixedUpdate(this as IFixedUpdate);
        }

        protected override void OnDestroy()
        {
            if (this is IUpdate) Engine.CETick.RmvUpdate(this as IUpdate);
            if (this is ILateUpdate) Engine.CETick.RmvLateUpdate(this as ILateUpdate);
            if (this is IFixedUpdate) Engine.CETick.RmvFixedUpdate(this as IFixedUpdate);

            base.OnDestroy();
        }
    }
}
