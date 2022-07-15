using GoblinFramework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    /// <summary>
    /// Goblin 对象，框架基础对象
    /// </summary>
    public abstract class Goblin
    {
        public GameEngineComp Engine;

        /// <summary>
        /// 创建一个 Goblin 对象
        /// </summary>
        /// <param name="goblin"></param>
        public virtual void Create(Goblin goblin)
        {
            this.Engine = goblin.Engine;
            OnCreate();
        }

        /// <summary>
        /// 销毁一个 Goblin 对象
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy();
        }

        /// <summary>
        /// 创建 Goblin 回调
        /// </summary>
        protected abstract void OnCreate();

        /// <summary>
        /// 销毁 Goblin 回调
        /// </summary>
        protected abstract void OnDestroy();
    }
}
