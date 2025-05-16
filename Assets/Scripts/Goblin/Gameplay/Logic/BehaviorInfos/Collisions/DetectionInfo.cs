using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;
using UnityEngine;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Collisions
{
    /// <summary>
    /// 碰撞结果
    /// </summary>
    public struct HitResult
    {
        /// <summary>
        /// 是否碰撞
        /// </summary>
        public bool hit { get; set; }
        /// <summary>
        /// 碰撞列表
        /// </summary>
        public List<(ulong id, FPVector3 point, FPVector3 normal, FP penetration)> colliders { get; set; }
    }
    
    /// <summary>
    /// 碰撞检测信息
    /// </summary>
    public class DetectionInfo : BehaviorInfo
    {
        protected override void OnReady()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }

        protected override BehaviorInfo OnClone()
        {
            throw new System.NotImplementedException();
        }
    }
}