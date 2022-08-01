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
    /// Sync-Model-Resolver，渲染指令模型解析
    /// </summary>
    public class SyncModelResolver : SyncResolver<SyncModelCmd>
    {
        public GameObject Model;
        public Animator Animator;

        protected override List<SyncCmd.CType> RelyResolvers => new List<SyncCmd.CType> { SyncCmd.CType.SyncAddCmd };

        protected async override void OnResolve<T>(T cmd)
        {
            if (null == Model) 
            {
                Model = await Engine.GameRes.Location.LoadActorPrefabAsync(cmd.modelName);
                Animator = Model.GetComponent<Animator>();
                SelfReady = true;
            }
        }
    }
}