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
            Theater = this;

            base.OnCreate();

            AddActor<Actors.Hoshi.HoshiActor>();
        }

        private int actorIdGen = -1;
        public int NewActorId
        {
            get
            {
                actorIdGen += 1;

                return actorIdGen;
            }
        }
    }
}
