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

        public GameObject CameraGo;

        protected override void OnCreate()
        {
            base.OnCreate();
            CameraGo = GameObject.Find("Eyes");
            CameraGo.transform.rotation = Quaternion.Euler(25, 0, 0);
        }

        private Vector3 offset = new Vector3(0f, 2f, -2.5f);
        public void LateUpdate(float tick)
        {
            if (null == FollowActor) return;

            var resolver = FollowActor.GetSyncResolver<SyncAddResolver>();
            var followPos = resolver.Node.transform.position + offset;

            CameraGo.transform.position = followPos;
        }
    }
}
