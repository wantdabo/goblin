using GoblinFramework.Core;
using GoblinFramework.Gameplay.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// Play-Comp，逻辑层组件
    /// </summary>
    public class PComp : Comp<PGEngine>
    {
        public Actor Actor;

        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IPLoop) Engine.TickEngine.AddPLoop(this as IPLoop);
            if (this is IPLateLoop) Engine.TickEngine.AddPLateLoop(this as IPLateLoop);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IPLoop) Engine.TickEngine.RmvPLoop(this as IPLoop);
            if (this is IPLateLoop) Engine.TickEngine.RmvPLateLoop(this as IPLateLoop);
        }

        public new T AddComp<T>(Action<T> createAheadAction = null) where T : PComp, new()
        {
            return base.AddComp<T>((item) =>
            {
                item.Actor = Actor;
                createAheadAction?.Invoke(item);
            });
        }
    }
}
