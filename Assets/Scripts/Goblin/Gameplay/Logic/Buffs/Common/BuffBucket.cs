using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Buffs.Common
{
    /// <summary>
    /// 触发 Buff 事件
    /// </summary>
    public struct BuffTriggerEvent : IEvent
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint id { get; set; }
    }

    /// <summary>
    /// 印下 Buff 事件
    /// </summary>
    public struct BuffEraseEvent : IEvent
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; set; }
    }

    /// <summary>
    /// 擦除 Buff 事件
    /// </summary>
    public struct BuffStampEvent : IEvent
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; set; }
    }

    /// <summary>
    /// Buff 桶
    /// </summary>
    public class BuffBucket : Behavior<Translator>
    {
        /// <summary>
        /// Buff 列表
        /// </summary>
        public List<uint> buffs { get; private set; } = new();
        /// <summary>
        /// Buff 字典
        /// </summary>
        private Dictionary<uint, Buff> buffdict { get; set; } = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<BuffTriggerEvent>(OnBuffTrigger);
            actor.eventor.Listen<BuffEraseEvent>(OnBuffErase);
            actor.eventor.Listen<BuffStampEvent>(OnBuffStamp);
            actor.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.eventor.UnListen<BuffTriggerEvent>(OnBuffTrigger);
            actor.eventor.UnListen<BuffEraseEvent>(OnBuffErase);
            actor.eventor.UnListen<BuffStampEvent>(OnBuffStamp);
            actor.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }
        
        /// <summary>
        /// 获取 Buff
        /// </summary>
        /// <param name="id">BuffID</param>
        /// <returns>Buff</returns>
        public Buff Get(uint id)
        {
            if (false == buffdict.TryGetValue(id, out var buff)) return default;

            return buff;
        }

        /// <summary>
        /// 获取 Buff
        /// </summary>
        /// <param name="id">BuffID</param>
        /// <typeparam name="T">Buff 类型</typeparam>
        /// <returns>Buff</returns>
        public T Get<T>(uint id) where T : Buff
        {
            var buff = Get(id);

            if (null == buff) return default;

            return buff as T;
        }
        
        private void OnBuffTrigger(BuffTriggerEvent e)
        {
            var buff = ReadyOrInitialize(e.id);
            buff.Trigger();
        }

        private void OnBuffErase(BuffEraseEvent e)
        {
            var buff = ReadyOrInitialize(e.id);
            buff.Erase(e.layer);
        }

        private void OnBuffStamp(BuffStampEvent e)
        {
            var buff = ReadyOrInitialize(e.id);
            buff.Stamp(e.layer);
        }

        private void OnFPTick(FPTickEvent e)
        {
            foreach (var buff in buffdict.Values)
            {
                if (BUFF_STATE_DEFINE.INACTIVE == buff.state) continue;
                buff.Execute(e.frame, e.tick);
            }
        }
        
        /// <summary>
        /// 就绪检查或者初始化 Buff
        /// </summary>
        /// <param name="id">BuffID</param>
        private Buff ReadyOrInitialize(uint id)
        {
            var buff = Get(id);
            if (null != buff) return buff;
            
            switch (id)
            {
                case BUFF_DEFINE.BUFF_10001:
                    buff = AddComp<BUFF_10001>();
                    break;
            }
            buff.bucket = this;
            buff.Create();
            buffdict.Add(id, buff);
            buffs.Add(id);

            return buff;
        }
    }
}
