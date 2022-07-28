using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    /// <summary>
    /// Behavior-Comp，战斗逻辑层行为组件
    /// </summary>
    /// <typeparam name="TI">组件数据类型</typeparam>
    public class Behavior<TI> : LComp where TI : LInfo, new()
    {
        private TI info = new TI();
        public TI Info { get { return info; } private set { info = value; } }
    }

    /// <summary>
    /// 组件数据定义，日后方便拷贝，预测状态
    /// </summary>
    public abstract class LInfo : Goblin<PGEngine>, ICloneable
    {
        protected override void OnCreate() { }
        protected override void OnDestroy() { }

        public abstract object Clone();
    }
}
