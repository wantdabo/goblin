using GoblinFramework.Common.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay.Resolves
{
    /// <summary>
    /// Sync-Resolver，渲染指令解析
    /// </summary>
    /// <typeparam name="RILT">指令类型</typeparam>
    public abstract class SyncResolver<RILT> : Resolver where RILT : RIL
    {
        protected override void OnPackResolver<T>(T ril)
        {
            OnResolve(ril as RILT);
        }

        protected abstract void OnResolve<T>(T ril) where T : RILT;
    }
}
