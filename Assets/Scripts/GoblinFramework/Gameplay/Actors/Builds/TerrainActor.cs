using BEPUutilities;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Builds
{
    public class TerrainActor : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            ActorBehavior.Info.pos = new Vector3(0, 0, 0);
            ActorBehavior.Info.scale = new Vector4(300, 1, 300, 1);
            AddComp<BoxCollider>();
        }
    }
}
