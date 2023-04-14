using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.Common.Events;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Render.Common
{
    /// <summary>
    /// Render-Tick, 渲染层 Tick 驱动组件
    /// </summary>
    public class Ticker : Comp
    {
        public Eventor eventor;

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
        }

        public void Update(float tick)
        {
            eventor.Tell(new UpdateEvent(){tick = tick});
        }
        
        public void LateUpdate(float tick)
        {
            eventor.Tell(new LateUpdateEvent(){tick = tick});
        }
        
        public void FixedUpdate(float tick)
        {
            eventor.Tell(new FixedUpdateEvent(){tick = tick});
        }
    }
}
