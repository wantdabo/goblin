using GoblinFramework.Gameplay.Actors.Hoshi.Controllers;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi
{
    public class HoshiBehavior : Behavior<HoshiBehavior.HoshiInfo>
    {
        public HoshiController Controller;

        protected override void OnCreate()
        {
            base.OnCreate();
            Controller = AddComp<HoshiController>();
        }

        #region
        public class HoshiInfo : LInfo
        {
            public override object Clone()
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
