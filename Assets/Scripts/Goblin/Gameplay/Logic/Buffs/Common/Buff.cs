using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Buffs.Common
{
    /// <summary>
    /// Buff
    /// </summary>
    public abstract class Buff : Comp
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public abstract uint id { get; }
        /// <summary>
        /// Buff 状态
        /// </summary>
        public byte state { get; private set; } = BUFF_STATE_DEFINE.INACTIVE;
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; private set; }
        /// <summary>
        /// Buff 最大层数
        /// </summary>
        public abstract uint maxlayer { get; }
        /// <summary>
        /// Buff 桶
        /// </summary>
        public BuffBucket bucket { get; set; }

        /// <summary>
        /// 活性检查
        /// </summary>
        private void Valid()
        {
            state = OnValid() ? BUFF_STATE_DEFINE.ACTIVE : BUFF_STATE_DEFINE.INACTIVE;
        }

        /// <summary>
        /// 触发 Buff
        /// </summary>
        public void Trigger()
        {
            OnTrigger();
        }

        /// <summary>
        /// 擦除 Buff
        /// </summary>
        /// <param name="layer">Buff 层数</param>
        public void Erase(uint layer = 1)
        {
            this.layer = Math.Clamp(this.layer - layer, 0, maxlayer);
            OnErase();
            Valid();
        }

        /// <summary>
        /// 印下 Buff
        /// </summary>
        /// <param name="layer">Buff 层数</param>
        public void Stamp(uint layer = 1)
        {
            this.layer = Math.Clamp(this.layer + layer, 0, maxlayer);
            OnStamp();
            Valid();
        }

        /// <summary>
        /// 执行 Buff
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="tick">tick</param>
        public void Execute(uint frame, FP tick)
        {
            OnExecute(frame, tick);
        }

        /// <summary>
        /// 活性检查
        /// </summary>
        /// <returns></returns>
        protected abstract bool OnValid();
        /// <summary>
        /// 触发 Buff
        /// </summary>
        protected virtual void OnTrigger() { }
        /// <summary>
        /// 擦除 Buff
        /// </summary>
        protected virtual void OnErase() { }
        /// <summary>
        /// 印下 Buff
        /// </summary>
        protected virtual void OnStamp() { }
        /// <summary>
        /// 执行 Buff
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="tick">tick</param>
        protected virtual void OnExecute(uint frame, FP tick) { }
    }
}
