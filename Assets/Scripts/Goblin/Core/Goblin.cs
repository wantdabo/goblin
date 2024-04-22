using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Core
{
    /// <summary>
    /// Goblin 对象，框架基础对象
    /// </summary>
    public abstract class Goblin
    {
        /// <summary>
        /// 销毁了
        /// </summary>
        public bool destroyed { get; private set; } = false;

        /// <summary>
        /// 创建一个 Goblin 对象
        /// </summary>
        public virtual void Create()
        {
            OnCreate();
        }

        /// <summary>
        /// 销毁一个 Goblin 对象
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy();
            destroyed = true;
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
