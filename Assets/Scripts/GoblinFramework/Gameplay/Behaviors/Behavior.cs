﻿using GoblinFramework.Core;
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
    /// Behavior，战斗逻辑层行为组件
    /// </summary>
    /// <typeparam name="I">组件数据类型</typeparam>
    public class Behavior<I> : PComp where I : BehaviorInfo, new()
    {
        private I info = null;
        public I Info { get { return info; } private set { info = value; } }

        protected override void OnCreate()
        {
            info = new I();
            info.Engine = Engine;
            info.Create();

            base.OnCreate();
        }
    }

    /// <summary>
    /// 组件数据定义
    /// </summary>
    public abstract class BehaviorInfo : Goblin<PGEngine>
    {
        protected override void OnCreate() { }
        protected override void OnDestroy() { }
    }
}