using GoblinFramework.Core;
using GoblinFramework.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Logic.Gameplay
{
    /// <summary>
    /// Behavior，战斗逻辑层行为组件
    /// </summary>
    public class Behavior : LComp
    {
        public Actor actor { get; set; }
    }

    /// <summary>
    /// Behavior，战斗逻辑层行为数据扩展组件
    /// </summary>
    /// <typeparam name="I">组件数据类型</typeparam>
    public class Behavior<I> : Behavior where I : BehaviorInfo, new()
    {
        private I mInfo = null;
        public I info { get { return mInfo; } private set { mInfo = value; } }

        protected override void OnCreate()
        {
            base.OnCreate();
            info = AddComp<I>();
            info.Create();
        }
    }

    /// <summary>
    /// 组件数据定义
    /// </summary>
    public abstract class BehaviorInfo : Comp { }
}
