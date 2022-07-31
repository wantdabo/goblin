using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay.Resolves;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Gameplay
{
    /// <summary>
    /// Actor, 演员
    /// </summary>
    public class Actor : CComp
    {
        public int actorId;
        public Theater Theater;
        private GameObject node;
        public GameObject Node { get { return node; } private set { node = value; } }

        protected override void OnCreate()
        {
            base.OnCreate();
            Node = new GameObject(actorId.ToString());
        }

        /// <summary>
        /// 指令解析器字典，用于根据指令做出相应行为，不可重复
        /// </summary>
        private Dictionary<SyncCmd.CType, CComp> syncResolverDict = new Dictionary<SyncCmd.CType, CComp>();

        /// <summary>
        /// 获取指令解析器
        /// </summary>
        /// <param name="type">指令解析器类型</param>
        /// <returns>指令解析器</returns>
        private CComp GetSyncResolver(SyncCmd.CType type)
        {
            syncResolverDict.TryGetValue(type, out CComp resolver);

            return resolver;
        }

        /// <summary>
        /// 设置一个指令解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        /// <param name="type">指令类型</param>
        private void SetSyncResolver<T, CT>(SyncCmd.CType type) where T : CComp, new() where CT : SyncCmd
        {
            var resolver = AddComp<T>();
            (resolver as SyncResolver<CT>).Actor = this;

            syncResolverDict.Add(type, resolver);
        }

        /// <summary>
        /// 检查指令解析器
        /// </summary>
        /// <param name="type">指令解析器类型</param>
        private void CheckSyncResolver(SyncCmd.CType type)
        {
            var resolver = GetSyncResolver(type);
            if (null != resolver) return;

            switch (type)
            {
                case SyncCmd.CType.SyncAddCmd:
                    SetSyncResolver<SyncAddResolver, SyncAddCmd>(type);
                    break;
                case SyncCmd.CType.SyncRmvCmd:
                    SetSyncResolver<SyncRmvResolver, SyncRmvCmd>(type);
                    break;
                case SyncCmd.CType.SyncPosCmd:
                    SetSyncResolver<SyncPosResolver, SyncPosCmd>(type);
                    break;
                case SyncCmd.CType.SyncStateCmd:
                    SetSyncResolver<SyncStateResolver, SyncStateCmd>(type);
                    break;
                case SyncCmd.CType.SyncModelCmd:
                    SetSyncResolver<SyncModelResolver, SyncModelCmd>(type);
                    break;
            }
        }

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="cmd">指令</param>
        public void Resolve<T>(T cmd) where T : SyncCmd
        {
            CheckSyncResolver(cmd.Type);
            var resolver = GetSyncResolver(cmd.Type);
            (resolver as SyncResolver<T>).Resolve<T>(cmd);
        }
    }
}
