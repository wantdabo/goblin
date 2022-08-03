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
    /// Sync-Remove-Resolver，渲染指令移除解析
    /// </summary>
    public class SyncRmvResolver : SyncResolver<RILRmv>
    {
        protected override List<RIL.RILType> RelyResolvers => new List<RIL.RILType> { RIL.RILType .RILAdd};

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        protected override void OnResolve<T>(T ril)
        {
            var resolver = Actor.GetSyncResolver<SyncAddResolver>();
            GameObject.Destroy(resolver.Node);
        }
    }
}