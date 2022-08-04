using Assets.Scripts.GoblinFramework.Client.Gameplay.Comps;
using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay.Resolves;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Gameplay.Comps
{
    public class CameraFollow : CPComp, ILateUpdate
    {
        public Actor FollowActor = null;

        private GameObject cameraGo;

        protected override void OnCreate()
        {
            base.OnCreate();
            cameraGo = GameObject.Find("Eyes");
        }

        private Vector3 offset = new Vector3(0.3f, 1.5f, 2);
        public void LateUpdate(float tick)
        {
            if (null == FollowActor) return;

            var resolver = FollowActor.GetSyncResolver<SyncAddResolver>();
            var followPos = resolver.Node.transform.position + offset;

            cameraGo.transform.position = followPos;
        }
    }
}
