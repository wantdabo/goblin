using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 碰撞指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class CollisionData : InstructData
    {
        public override ushort id => INSTR_DEFINE.COLLISION;

        /// <summary>
        /// 类型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetCollisionTypeDefine()")]
        [LabelText("类型")]
        public byte type = COLLISION_DEFINE.COLLISION_TYPE_HURT;
        
        /// <summary>
        /// 检测类型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetCollisionOverlapDefine()")]
        [LabelText("检测类型")]
        public byte overlaptype = COLLISION_DEFINE.COLLISION_BOX;
        /// <summary>
        /// 最大检测次数
        /// </summary>
        [LabelText("检测次数")]
        public uint count = 1;
        /// <summary>
        /// 偏移
        /// </summary>
        [LabelText("偏移")]
        public IntVector3 offset;
        /// <summary>
        /// 射线方向
        /// </summary>
        [ShowIf("@COLLISION_DEFINE.COLLISION_RAY == overlaptype")]
        [LabelText("射线方向")]
        public IntVector3 raydire = new(0, 0, 1000);
        /// <summary>
        /// 射线长度
        /// </summary>
        [ShowIf("@COLLISION_DEFINE.COLLISION_RAY == overlaptype")]
        [LabelText("射线长度")]
        public uint raydis = 1000;
        /// <summary>
        /// 线段终点
        /// </summary>
        [ShowIf("@COLLISION_DEFINE.COLLISION_LINE == overlaptype")]
        [LabelText("线段终点")]
        public IntVector3 lineep = new(0, 0, 1000);
        /// <summary>
        /// 立方体大小
        /// </summary>
        [LabelText("立方体大小")]
        [ShowIf("@COLLISION_DEFINE.COLLISION_BOX == overlaptype")]
        public IntVector3 boxsize = new(1000, 1000, 1000);
        /// <summary>
        /// 球体半径
        /// </summary>
        [LabelText("球体半径")]
        [ShowIf("@COLLISION_DEFINE.COLLISION_SPHERE == overlaptype")]
        public uint sphereradius = 500;
        /// <summary>
        /// 使用[自身]命中火花
        /// </summary>
        [LabelText("使用[自身]命中火花")]
        [BoxGroup("使用命中火花")]
        [GUIColor(0.7f, 0.7f, 1f)]
        [ShowIf("@COLLISION_DEFINE.COLLISION_TYPE_HURT == type")]
        public bool usesparkself;
        /// <summary>
        /// 使用[目标]命中火花
        /// </summary>
        [LabelText("使用[目标]命中火花")]
        [BoxGroup("使用命中火花")]
        [GUIColor(0.7f, 0.7f, 1f)]
        [ShowIf("@COLLISION_DEFINE.COLLISION_TYPE_HURT == type")]
        public bool usesparktarget;
        /// <summary>
        /// [自身]命中火花
        /// </summary>
        [HideLabel]
        [ShowIf("@COLLISION_DEFINE.COLLISION_TYPE_HURT == type && usesparkself")]
        [BoxGroup("[自身]命中火花")]
        [InlineProperty]
        [HideReferenceObjectPicker]
        public SparkData sparkself;
        /// <summary>
        /// [目标]命中火花
        /// </summary>
        [HideLabel]
        [ShowIf("@COLLISION_DEFINE.COLLISION_TYPE_HURT == type && usesparktarget")]
        [BoxGroup("[目标]命中火花")]
        [InlineProperty]
        [HideReferenceObjectPicker]
        public SparkData sparktarget;
    }
}