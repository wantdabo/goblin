using GoblinFramework.Client.Common;
using GoblinFramework.Client.Gameplay.Resolves;
using GoblinFramework.General.Gameplay.RIL.RILS;
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

        protected override void OnCreate()
        {
            base.OnCreate();
            SetSyncResolver<SyncAddResolver, RILAdd>(RIL.RILType.RILAdd);
            SetSyncResolver<SyncRmvResolver, RILRmv>(RIL.RILType.RILRmv);
            SetSyncResolver<SyncPosResolver, RILPos>(RIL.RILType.RILPos);
            SetSyncResolver<SyncModelResolver, RILModel>(RIL.RILType.RILModel);
            SetSyncResolver<SyncStateResolver, RILState>(RIL.RILType.RILState);
        }

        /// <summary>
        /// 指令解析器字典，用于根据指令做出相应行为，不可重复
        /// </summary>
        private Dictionary<RIL.RILType, Resolver> syncResolverDict = new Dictionary<RIL.RILType, Resolver>();
        private Dictionary<Type, Resolver> syncResolverTypeDict = new Dictionary<Type, Resolver>();

        /// <summary>
        /// 获取指令解析器
        /// </summary>
        /// <param name="type">指令类型</param>
        /// <returns>指令解析器，装箱的</returns>
        public Resolver GetSyncResolver(RIL.RILType type)
        {
            syncResolverDict.TryGetValue(type, out Resolver resolver);

            return resolver;
        }

        /// <summary>
        /// 获取指令解析器
        /// </summary>
        /// <typeparam name="T">指令解析器类型</typeparam>
        /// <returns>指令解析器，拆箱的</returns>
        public T GetSyncResolver<T>() where T : Resolver
        {
            if (syncResolverTypeDict.TryGetValue(typeof(T), out var comp)) return comp as T;

            return null;
        }

        /// <summary>
        /// 设置一个指令解析器
        /// </summary>
        /// <typeparam name="SRT">解析器类型</typeparam>
        /// <param name="type">指令类型</param>
        private void SetSyncResolver<SRT, RILT>(RIL.RILType type) where SRT : Resolver, new() where RILT : RIL
        {
            var resolver = AddComp<SRT>((item) => { (item as SyncResolver<RILT>).Actor = this; });

            syncResolverDict.Add(type, resolver);
            syncResolverTypeDict.Add(typeof(SRT), resolver);
        }

        /// <summary>
        /// 指令解析方法
        /// </summary>
        /// <typeparam name="T">指令类型</typeparam>
        /// <param name="ril">指令</param>
        public void Resolve<T>(T ril) where T : RIL
        {
            GetSyncResolver(ril.Type).Resolve(ril);
        }
    }
}
