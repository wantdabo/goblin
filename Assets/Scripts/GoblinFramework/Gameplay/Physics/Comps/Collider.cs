using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Comps
{
    public abstract class Collider : PComp
    {
        protected Entity body;
        public Vector3 pos { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            RegisterDetectBody();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnRegisterDetectBody();
        }

        private void RegisterDetectBody()
        {
            //body.CollisionInformation.Events.
        }

        private void UnRegisterDetectBody()
        {

        }
    }
}
