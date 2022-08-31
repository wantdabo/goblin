using GoblinFramework.Common.Gameplay.RIL.RILS;
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
    public class SyncModelResolver : SyncResolver<RILModel>
    {
        public GameObject Model;
        public Animator Animator;

        protected override List<RIL.RILType> RelyResolvers => new List<RIL.RILType> { RIL.RILType.RILAdd };

        protected async override void OnResolve<T>(T ril)
        {
            if (null == Model)
            {
                Model = await Engine.GameRes.Location.LoadActorPrefabAsync(ril.modelName);
                Model.transform.SetParent(Actor.GetSyncResolver<SyncAddResolver>().Node.transform);
                Animator = Model.GetComponent<Animator>();
                SelfReady = true;
            }
        }
    }
}