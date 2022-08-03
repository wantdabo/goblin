using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.RIL.RILS;
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
    public abstract class SyncResolver<RILT> : CComp
    {
        public Actor Actor;

        /// <summary>
        /// 依赖检查
        /// </summary>
        protected abstract List<RIL.RILType> RelyResolvers { get; }

        /// <summary>
        /// 渲染指令
        /// </summary>
        private RILT cacheRil;

        /// <summary>
        /// 自身否就绪检查/供其他解析器解读是否就绪
        /// </summary>
        private bool selfReady = false;
        protected bool SelfReady
        {
            get { return selfReady; }
            set
            {
                selfReady = value;

                if (false == value) return;

                foreach (var action in selfReadyInterestedActionList) action?.Invoke();
            }
        }

        /// <summary>
        /// 全部依赖就绪
        /// </summary>
        private bool relyReady = false;
        protected bool RelyReady { get { return relyReady; } private set { relyReady = value; } }

        private List<Action> selfReadyInterestedActionList = new List<Action>();
        private void RegisterInterestedReady(Action action)
        {
            selfReadyInterestedActionList.Add(action);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            foreach (var r in RelyResolvers)
            {
                var resolver = Actor.GetSyncResolver(r) as SyncResolver<RILT>;
                resolver.RegisterInterestedReady(() =>
                {
                    if (null != cacheRil || false == CheckRelyResolvers()) Resolve(cacheRil);
                });
            }
        }

        /// <summary>
        /// 依赖是否就绪检查
        /// </summary>
        /// <returns>就绪 / true 表示就绪，false 表示未就绪</returns>
        private bool CheckRelyResolvers()
        {
            if (null == RelyResolvers) { RelyReady = true; return RelyReady; };

            foreach (var r in RelyResolvers)
            {
                var resolver = Actor.GetSyncResolver(r) as SyncResolver<RILT>;
                if (false == resolver.SelfReady) return false;
            }

            RelyReady = true;
            return RelyReady;
        }

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="ril">指令</param>
        public void Resolve<T>(T ril) where T : RILT
        {
            if (false == RelyReady && false == CheckRelyResolvers())
            {
                cacheRil = ril;

                return;
            }

            OnResolve(ril);
        }

        protected abstract void OnResolve<T>(T ril) where T : RILT;
    }
}
