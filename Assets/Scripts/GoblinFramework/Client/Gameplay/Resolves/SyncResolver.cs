using GoblinFramework.Client.Common;
using GoblinFramework.General.Gameplay.Command.Cmds;
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
    /// <typeparam name="CT">指令类型</typeparam>
    public abstract class SyncResolver<CT> : CComp
    {
        public Actor Actor;

        /// <summary>
        /// 依赖检查
        /// </summary>
        protected abstract List<SyncCmd.CType> RelyResolvers { get; }

        /// <summary>
        /// 渲染指令
        /// </summary>
        private CT cacheCmd;

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
                var resolver = Actor.GetSyncResolver(r) as SyncResolver<CT>;
                resolver.RegisterInterestedReady(() =>
                {
                    if (null != cacheCmd || false == CheckRelyResolvers()) Resolve(cacheCmd);
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
                var resolver = Actor.GetSyncResolver(r) as SyncResolver<CT>;
                if (false == resolver.SelfReady) return false;
            }

            RelyReady = true;
            return RelyReady;
        }

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="cmd">指令</param>
        public void Resolve<T>(T cmd) where T : CT
        {
            if (false == RelyReady && false == CheckRelyResolvers())
            {
                cacheCmd = cmd;

                return;
            }

            OnResolve(cmd);
        }

        protected abstract void OnResolve<T>(T cmd) where T : CT;
    }
}
