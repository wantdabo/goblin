using BEPUutilities;
using GoblinFramework.Gameplay.Physics.Comps;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Builds
{
    public class CubeActor : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            ActorBehavior.SendRIL<RILModel>((ril) => ril.modelName = "Builds/Cube");
            ActorBehavior.Info.scale = new Vector4(1, 1, 1, 1);
            AddComp<BoxCollider>();
        }
    }
}
