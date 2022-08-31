using BEPUutilities;
using GoblinFramework.Common.Gameplay.RIL.RILS;
using GoblinFramework.Gameplay.Physics.Comps;
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
            actorBehaivor.SendRIL<RILModel>((ril) => ril.modelName = "Builds/Cube");
            actorBehaivor.info.scale = new Vector4(1, 4, 1, 1);
            AddComp<BoxCollider>();
        }
    }
}
