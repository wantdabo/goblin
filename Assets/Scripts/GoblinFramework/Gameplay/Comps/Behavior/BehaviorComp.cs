using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Comps
{
    /// <summary>
    /// Behavior-Comp，战斗逻辑层行为组件
    /// </summary>
    /// <typeparam name="T">组件数据类型</typeparam>
    public class BehaviorComp<T> : LComp where T : LInfo, new()
    {
        private T info = new T();
        public T Info { get { return info; } private set { info = value; } }
    }

    /// <summary>
    /// 组件数据定义，日后方便拷贝，预测状态
    /// </summary>
    public abstract class LInfo : ICloneable
    {
        public abstract object Clone();
    }
}
