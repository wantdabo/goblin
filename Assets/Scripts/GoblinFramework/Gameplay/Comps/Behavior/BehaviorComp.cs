using GoblinFramework.Core;
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
    /// <typeparam name="TI">组件数据类型</typeparam>
    public class BehaviorComp<TI> : LComp where TI : LInfo, new()
    {
        private TI info = new TI();
        public TI Info { get { return info; } private set { info = value; } }

        /// <summary>
        /// 禁用 RmvComp，如果需要添加组件，请使用 Actor.RmvBehavior/Actor.RmvActor
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>Exception</returns>
        /// <exception cref="Exception">请使用 Actor.RmvBehavior/Actor.RmvActor 去完成工作</exception>
        public override void RmvComp<T>()
        {
            throw new Exception("plz use Actor.RmvBehavior/Actor.RmvActor to finish your work.");
        }

        /// <summary>
        /// 禁用 RmvComp，如果需要添加组件，请使用 Actor.RmvBehavior/Actor.RmvActor
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>Exception</returns>
        /// <exception cref="Exception">请使用 Actor.RmvBehavior/Actor.RmvActor 去完成工作</exception>
        public override void RmvComp(Comp<PGEngine> comp)
        {
            throw new Exception("plz use Actor.RmvBehavior/Actor.RmvActor to finish your work.");
        }

        /// <summary>
        /// 禁用 GetComp，如果需要添加组件，请使用 Actor.GetBehavior/Actor.GetActor
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>Exception</returns>
        /// <exception cref="Exception">请使用 Actor.GetBehavior/Actor.GetActor 去完成工作</exception>
        public override List<T> GetComp<T>(bool force = false)
        {
            throw new Exception("plz use Actor.GetBehavior/Actor.GetActor to finish your work.");
        }

        /// <summary>
        /// 禁用 AddComp，如果需要添加组件，请使用 Actor.AddBehavior/Actor.AddActor
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>Exception</returns>
        /// <exception cref="Exception">请使用 Actor.AddBehavior/AddActor 去完成工作</exception>
        public override T AddComp<T>()
        {
            throw new Exception("plz use Actor.AddBehavior/Actor.AddActor to finish your work.");
        }
    }

    /// <summary>
    /// 组件数据定义，日后方便拷贝，预测状态
    /// </summary>
    public abstract class LInfo : ICloneable
    {
        public abstract object Clone();
    }
}
