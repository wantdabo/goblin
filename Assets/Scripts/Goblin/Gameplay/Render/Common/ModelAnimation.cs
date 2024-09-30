using Goblin.Common;
using Goblin.Gameplay.Common;
using Goblin.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 播放动画事件
    /// </summary>
    public struct PlayAnimEvent : IEvent
    {
        /// <summary>
        /// 动画名
        /// </summary>
        public string animName;
        /// <summary>
        /// 循环
        /// </summary>
        public bool loop;
        /// <summary>
        /// 动画状态机层级
        /// </summary>
        public int layer;
    }

    /// <summary>
    /// 动画播放器
    /// </summary>
    public abstract class ModelAnimation : Behavior
    {
        /// <summary>
        /// 当前播放的动画名（下标对应层级，数据表示动画名）
        /// </summary>
        public string[] animNames { get; protected set; } = new string[2];
        /// <summary>
        /// 当前播放的动画，是否循环
        /// </summary>
        public bool[] loops { get; protected set; } = new bool[2];
    }
}
