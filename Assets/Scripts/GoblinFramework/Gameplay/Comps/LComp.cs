using GoblinFramework.Core;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Comps
{
    /// <summary>
    /// Logic-Comp，战斗逻辑层组件
    /// </summary>
    /// <typeparam name="T">组件数据类型</typeparam>
    public class LComp<T> : Comp<PGEngine> where T : LInfo, new()
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
