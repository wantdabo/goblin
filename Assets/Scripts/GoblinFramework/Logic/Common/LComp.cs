using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Logic.Common
{
    public class LComp : Comp<LGEngine>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is ILoop) Engine.Ticker.AddLoop(this as ILoop);
            if (this is ILateLoop) Engine.Ticker.AddLateLoop(this as ILateLoop);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is ILoop) Engine.Ticker.RmvLoop(this as ILoop);
            if (this is ILateLoop) Engine.Ticker.RmvLateLoop(this as ILateLoop);
        }
    }
}
