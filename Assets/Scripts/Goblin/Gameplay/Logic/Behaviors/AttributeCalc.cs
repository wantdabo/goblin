using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 伤害的数据结构
    /// </summary>
    public struct DamageInfo
    {
        /// <summary>
        /// 暴击
        /// </summary>
        public bool crit { get; set; }
        /// <summary>
        /// 伤害数值
        /// </summary>
        public uint value { get; set; }
    }
    
    /// <summary>
    /// 属性数值计算
    /// </summary>
    public class AttributeCalc : Behavior
    {
        /// <summary>
        /// 计算伤害数值
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="strength">伤害强度</param>
        /// <returns>伤害</returns>
        public DamageInfo ChargeDamage(ulong id, FP strength)
        {
            if (false == stage.SeekBehaviorInfo(id, out AttributeInfo attribute)) return default;
            
            DamageInfo damage = new()
            {
                crit = false,
                value = FP.ToUInt(strength * attribute.attack)
            };

            return damage;
        }
        
        /// <summary>
        /// 计算抵抗后的伤害数值
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="damage">伤害</param>
        /// <returns>抵抗后的伤害</returns>
        public DamageInfo DischargeDamage(ulong id, DamageInfo damage)
        {
            if (false == stage.SeekBehaviorInfo(id, out AttributeInfo attribute)) return damage;

            return damage;
        }

        /// <summary>
        /// 施加伤害
        /// </summary>
        /// <param name="from">来源</param>
        /// <param name="to">去向</param>
        /// <param name="damage">伤害</param>
        public void ToDamage(ulong from, ulong to, DamageInfo damage)
        {
            if (false == stage.SeekBehaviorInfo(from, out AttributeInfo fromattribute)) return;
            if (false == stage.SeekBehaviorInfo(to, out AttributeInfo toattribute)) return;
            
            // TODO 伤害结算
        }
    }
}