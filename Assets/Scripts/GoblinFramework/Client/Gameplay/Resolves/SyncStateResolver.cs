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
    /// Sync-State-Resolver，渲染指令动画状态解析
    /// </summary>
    public class SyncStateResolver : SyncResolver<RILState>
    {
        protected override List<RIL.RILType> RelyResolvers => new List<RIL.RILType> { RIL.RILType.RILModel };

        protected override void OnCreate()
        {
            base.OnCreate();
            SelfReady = true;
        }

        protected override void OnResolve<T>(T ril)
        {
            var modelResolver = Actor.GetSyncResolver<SyncModelResolver>();

            foreach (var paramter in modelResolver.Animator.parameters)
                if (AnimatorControllerParameterType.Bool == paramter.type) modelResolver.Animator.SetBool(paramter.name, paramter.defaultBool);

            modelResolver.Animator.SetBool(ril.stateName, true);
        }
    }
}