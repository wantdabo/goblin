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
    /// Sync-Remove-Resolver，渲染指令移除解析
    /// </summary>
    public class SyncRmvResolver : SyncResolver<SyncRmvCmd>
    {
        protected override List<SyncCmd.CType> RelyResolvers => new List<SyncCmd.CType> { SyncCmd.CType .SyncAddCmd};

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        protected override void OnResolve<T>(T cmd)
        {
            var resolver = Actor.GetSyncResolver<SyncAddResolver>();
            GameObject.Destroy(resolver.Node);
        }
    }
}