using GoblinFramework.Gameplay.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Theaters
{
    public class Theater : Actor
    {
        protected override void OnCreate()
        {
            Actor = this;
            base.OnCreate();
            AddActor<Actors.Hoshi.HoshiActor>();
        }
    }
}
