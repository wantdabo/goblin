﻿namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// RIL 定义
    /// </summary>
    public class RIL_DEFINE
    {
        /// <summary>
        /// LIVE 出生指令
        /// </summary>
        public const ushort LIVE_BORN = 0;
        /// <summary>
        /// LIVE 死亡指令
        /// </summary>
        public const ushort LIVE_DEAD = 1;
        /// <summary>
        /// SPATIAL 平移指令
        /// </summary>
        public const ushort SPATIAL_POSITION = 2;
        /// <summary>
        /// SPATIAL 旋转指令
        /// </summary>
        public const ushort SPATIAL_ROTATION = 3;
        /// <summary>
        /// SPATIAL 缩放指令
        /// </summary>
        public const ushort SPATIAL_SCALE = 4;
        /// <summary>
        /// STATE_MACHINE ZERO 层状态机指令
        /// </summary>
        public const ushort STATE_MACHINE_ZERO = 5;
        /// <summary>
        /// STATE_MACHINE ONE 层状态机指令
        /// </summary>
        public const ushort STATE_MACHINE_ONE = 6;
        /// <summary>
        /// SKILL_PIPELINE_INFO 技能管线信息指令
        /// </summary>
        public const ushort SKILL_PIPELINE_INFO = 7;
        /// <summary>
        /// BULLET_INFO 技能子弹信息指令
        /// </summary>
        public const ushort BULLET_INFO = 8;
        /// <summary>
        /// BUFF_INFO BUFF 信息指令
        /// </summary>
        public const ushort BUFF_INFO = 9;
        /// <summary>
        /// RECV_CURE_INFO 受到治疗信息指令
        /// </summary>
        public const ushort RECV_CURE_INFO = 10;
        /// <summary>
        /// RECV_HURT_INFO 受到伤害信息指令
        /// </summary>
        public const ushort RECV_HURT_INFO = 11;
        /// <summary>
        /// ATTRIBUTE 生命值指令
        /// </summary>
        public const ushort ATTRIBUTE_HP = 12;
        /// <summary>
        /// ATTRIBUTE 最大生命值指令
        /// </summary>
        public const ushort ATTRIBUTE_MAXHP = 13;
        /// <summary>
        /// ATTRIBUTE 移动速度指令
        /// </summary>
        public const ushort ATTRIBUTE_MOVESPEED = 14;
        /// <summary>
        /// ATTRIBUTE 攻击力指令
        /// </summary>
        public const ushort ATTRIBUTE_ATTACK = 15;
        /// <summary>
        /// SURFACE 指令
        /// </summary>
        public const ushort SURFACE = 16;
    }
}
