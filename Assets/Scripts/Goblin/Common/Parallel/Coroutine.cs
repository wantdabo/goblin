using Goblin.Common.Parallel.Instructions;
using System.Collections;
using System.Collections.Generic;

namespace Goblin.Common.Parallel
{
    /// <summary>
    /// 协程
    /// </summary>
    public class Coroutine
    {
        /// <summary>
        /// ID
        /// </summary>
        public uint id { get; private set; }
        /// <summary>
        /// 协程驱动器
        /// </summary>
        private CoroutineScheduler coroutines { get; set; }
        /// <summary>
        /// 逻辑分片
        /// </summary>
        private IEnumerator<Instruction> enumerator { get; set; }
        
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="id">协程 ID</param>
        /// <param name="coroutines">协程驱动</param>
        /// <param name="enumerator">逻辑分片</param>
        public void Reset(uint id, CoroutineScheduler coroutines, IEnumerator<Instruction> enumerator)
        {
            this.id = id;
            this.coroutines = coroutines;
            this.enumerator = enumerator;
        }
        
        /// <summary>
        /// 协程指令就绪检查
        /// </summary>
        /// <returns>YES/NO</returns>
        public bool Ready()
        {
            if (null == enumerator.Current) return true;

            return enumerator.Current.finished;
        }

        /// <summary>
        /// 驱动协程指令
        /// </summary>
        /// <param name="tick">tick</param>
        public void Update(float tick)
        {
            if (null == enumerator.Current) return;
            enumerator.Current.Update(tick);
        }
        
        /// <summary>
        /// 执行协程
        /// </summary>
        /// <returns>YES/NO (true 表示逻辑分片未结束/ 否则就是逻辑分片已结束)</returns>
        public bool Execute()
        {
            return enumerator.MoveNext();
        }
    }
}
