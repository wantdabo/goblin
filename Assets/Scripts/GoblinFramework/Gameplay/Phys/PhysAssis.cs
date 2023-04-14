using System.Collections;
using System.Collections.Generic;
using GoblinFramework.Core;
using UnityEngine;

namespace GoblinFramework.Gameplay.Phys
{
    public class PhysAssisInfo : BehaviorInfo
    {
        public PhysXU3D physxU3D;
    }

    public class PhysAssis : Behavior<PhysAssisInfo>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}