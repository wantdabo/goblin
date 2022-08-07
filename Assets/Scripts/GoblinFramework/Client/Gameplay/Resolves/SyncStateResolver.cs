using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            General.GoblinDebug.Log(ril.stateId.ToString());
            modelResolver.Animator.SetBool("Idle", false);
            modelResolver.Animator.SetBool("Run", false);
            modelResolver.Animator.SetBool("AttackA", false);
            modelResolver.Animator.SetBool("AttackB", false);
            modelResolver.Animator.SetBool("AttackCHold", false);
            modelResolver.Animator.SetBool("AttackC", false);

            if (1 == ril.stateId)
                modelResolver.Animator.SetBool("Idle", true);
            else if (2 == ril.stateId)
                modelResolver.Animator.SetBool("Run", true);
            else if (3 == ril.stateId)
                modelResolver.Animator.SetBool("AttackA", true);
            else if (4 == ril.stateId)
                modelResolver.Animator.SetBool("AttackB", true);
            else if (5 == ril.stateId)
                modelResolver.Animator.SetBool("AttackCHold", true);
            else if (6 == ril.stateId)
                modelResolver.Animator.SetBool("AttackC", true);
        }
    }
}