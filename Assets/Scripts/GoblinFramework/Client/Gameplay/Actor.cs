using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay.Resolves;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client.Gameplay
{
    /// <summary>
    /// Actor, 演员
    /// </summary>
    public class Actor : CComp
    {
        public Theater Theater;

        public int actorId;

        /// <summary>
        /// 指令解析器字典，用于根据指令做出相应行为，不可重复
        /// </summary>
        private Dictionary<SyncCmd.CType, SyncResolver> syncResolverDict = new Dictionary<SyncCmd.CType, SyncResolver>();

        /// <summary>
        /// 获取指令解析器
        /// </summary>
        /// <param name="type">指令解析器类型</param>
        /// <returns>指令解析器</returns>
        private SyncResolver GetSyncResolver(SyncCmd.CType type)
        {
            syncResolverDict.TryGetValue(type, out SyncResolver resolver);

            return resolver;
        }

        private void SetSyncResolver<T>(SyncCmd.CType type) where T : SyncResolver, new()
        {
            var resolver = AddComp<T>();
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
                    SetSyncResolver<SyncAddResolver>(type);
                    break;
                case SyncCmd.CType.SyncRmvCmd:
                    SetSyncResolver<SyncRmvResolver>(type);
                    break;
                case SyncCmd.CType.SyncPosCmd:
                    SetSyncResolver<SyncPosResolver>(type);
                    break;
                case SyncCmd.CType.SyncStateCmd:
                    SetSyncResolver<SyncStateResolver>(type);
                    break;
                case SyncCmd.CType.SyncModelCmd:
                    SetSyncResolver<SyncModelResolver>(type);
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
        }
    }
}
