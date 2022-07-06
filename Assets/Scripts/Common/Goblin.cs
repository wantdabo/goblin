using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common
{
    /// <summary>
    /// Goblin 对象，框架基础对象
    /// </summary>
    internal abstract class Goblin
    {
        /// <summary>
        /// 创建一个 Goblin 对象
        /// </summary>
        public virtual void Create(){ OnCreate(); }

        /// <summary>
        /// 销毁一个 Goblin 对象
        /// </summary>
        public virtual void Destroy() { OnDestroy(); }

        /// <summary>
        /// 创建 Goblin 回调
        /// </summary>
        public abstract void OnCreate();

        /// <summary>
        /// 销毁 Goblin 回调
        /// </summary>
        public abstract void OnDestroy();
    }
}
