using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.Command.Cmds;
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
    public class SyncPosResolver : SyncResolver<SyncPosCmd>, IUpdate
    {
        protected override List<SyncCmd.CType> RelyResolvers => new List<SyncCmd.CType> { SyncCmd.CType.SyncAddCmd };

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        private Vector3 lerp2Target;
        private float lerp2dire;
        protected override void OnResolve<T>(T cmd)
        {
            lerp2Target.Set(cmd.x, 0, cmd.y);
            lerp2dire = cmd.dire;
        }

        public void Update(float tick)
        {
            var trans = Actor.GetSyncResolver<SyncAddResolver>().Node.transform;

            // 坐标算法，插值算法后边再写
            trans.position = Vector3.Lerp(trans.position, lerp2Target, tick);

            // 旋转算法，插值算法后边再写
            var eulerAngles = trans.rotation.eulerAngles;
            eulerAngles.y = Mathf.Lerp(eulerAngles.y, lerp2dire, tick);
            trans.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }
    }
}