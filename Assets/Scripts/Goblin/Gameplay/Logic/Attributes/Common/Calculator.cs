using Goblin.Core;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Attributes.Common
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
    /// 数值计算器
    /// </summary>
    public class Calculator : Comp
    {
        /// <summary>
        /// 属性集合
        /// </summary>
        public Dictionary<uint, Attribute> attributedict { get; private set; } = new();

        /// <summary>
        /// 将目标属性从战斗计算器中移除
        /// </summary>
        /// <param name="attribute">目标属性</param>
        public void UnRegister(Attribute attribute)
        {
            if (false == attributedict.ContainsKey(attribute.actor.id)) return;
            attributedict.Remove(attribute.actor.id);
        }

        /// <summary>
        /// 将目标属性添加至战斗计算器中
        /// </summary>
        /// <param name="attribute">目标属性</param>
        public void Register(Attribute attribute)
        {
            if (attributedict.ContainsKey(attribute.actor.id)) return;
            
            attributedict.Add(attribute.actor.id, attribute);
        }
        
        /// <summary>
        /// 计算抵抗后的伤害数值
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="damage">伤害</param>
        /// <returns>抵抗后的伤害</returns>
        public DamageInfo DischargeDamage(uint actorId, DamageInfo damage)
        {
            if (false == attributedict.TryGetValue(actorId, out var attribute)) return damage;

            return damage;
        }

        /// <summary>
        /// 计算伤害数值
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="rate">伤害强度</param>
        /// <returns>伤害</returns>
        public DamageInfo ChargeDamage(uint actorId, FP rate)
        {
            if (false == attributedict.TryGetValue(actorId, out var attribute)) return default;
            DamageInfo damage = new()
            {
                crit = false,
                value = FP.ToUInt(rate * attribute.attack)
            };

            return damage;
        }
    }
}
