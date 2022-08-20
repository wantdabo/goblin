using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    /// <summary>
    /// Sync-Positon-Resolver，渲染指令坐标、旋转解析
    /// </summary>
    public class SyncPosResolver : SyncResolver<RILPos>, IUpdate
    {
        protected override List<RIL.RILType> RelyResolvers => new List<RIL.RILType> { RIL.RILType.RILAdd };

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        private Vector3 lerp2Pos;
        private float lerp2dire;
        protected override void OnResolve<T>(T ril)
        {
            lerp2Pos.Set(ril.x, ril.y, ril.z);
            var trans = Actor.GetSyncResolver<SyncAddResolver>().Node.transform;

            // 角度计算
            var dire = trans.position - lerp2Pos;
            float radian = Mathf.Atan2(dire.z, -dire.x);
            lerp2dire = radian * 180 / Mathf.PI + 90;
        }

        public void Update(float tick)
        {
            if (false == RelyReady) return;

            var trans = Actor.GetSyncResolver<SyncAddResolver>().Node.transform;

            // 坐标算法，插值算法后边再写
            trans.position = Vector3.Lerp(trans.position, lerp2Pos, tick * 15);
            //trans.position = lerp2Pos;

            // 旋转算法，插值算法后边再写
            var eulerAngles = trans.rotation.eulerAngles;
            eulerAngles.y = Mathf.LerpAngle(eulerAngles.y, lerp2dire, tick * 5);

            trans.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }
    }
}