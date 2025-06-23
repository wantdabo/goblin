using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.EVENT;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
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
        public int value { get; set; }
    }
    
    /// <summary>
    /// 属性数值计算
    /// </summary>
    public class AttributeCalc : Behavior
    {
        /// <summary>
        /// 转换属性键值
        /// </summary>
        /// <param name="key">属性 Key</param>
        /// <returns>(主 Key, 千分比 Key)</returns>
        private (ushort mainkey, ushort scalekey) ConvAttribyteKey(ushort key)
        {
            return ((ushort)(key * 2 + 1), (ushort)(key * 2 + 2));
        }
        
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <returns>数值</returns>
        public int GetAttributeValue(AttributeInfo attribute, ushort key)
        {
            var attrkey = ConvAttribyteKey(key);
            var mainvalue = attribute.baseattributes.GetValueOrDefault(attrkey.mainkey, 0);
            var mainscalevalue = attribute.baseattributes.GetValueOrDefault(attrkey.scalekey, 1000);
            var addivalue = attribute.addiattributes.GetValueOrDefault(attrkey.mainkey, 0);
            var addiscalevalue = attribute.addiattributes.GetValueOrDefault(attrkey.scalekey, 1000);
            
            var scale = (mainscalevalue + addiscalevalue) * FP.Half * stage.cfg.int2fp;
            return ((mainvalue + addivalue) * scale).AsInt();
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <param name="value">数值</param>
        public void SetAttributeValue(AttributeInfo attribute, ushort key, int value)
        {
            var attrkey = ConvAttribyteKey(key);
            if (attribute.baseattributes.ContainsKey(attrkey.mainkey)) attribute.baseattributes.Remove(attrkey.mainkey);
            attribute.baseattributes.Add(attrkey.mainkey, value);
        }

        /// <summary>
        /// 设置属性的千分比值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <param name="value">数值</param>
        /// <exception cref="Exception">不能设置 HP 的千分比|千分比数值必须大于 0</exception>
        public void SetAttributeScaleValue(AttributeInfo attribute, ushort key, int value)
        {
            if (ATTRIBUTE_DEFINE.HP == key) throw new Exception("cannot set HP as scale value");
            if (0 >= value) throw new Exception("scale value must be greater than 0");
            
            var attrkey = ConvAttribyteKey(key);
            if (attribute.baseattributes.ContainsKey(attrkey.scalekey)) attribute.baseattributes.Remove(attrkey.scalekey);
            attribute.baseattributes.Add(attrkey.scalekey, value);
        }

        /// <summary>
        /// 计算伤害数值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="strength">伤害强度</param>
        /// <returns>伤害</returns>
        public DamageInfo ChargeDamage(ulong actor, FP strength)
        {
            if (false == stage.SeekBehaviorInfo(actor, out AttributeInfo attribute)) return default;
            DamageInfo damage = new()
            {
                crit = false,
                value = FP.ToInt(strength * GetAttributeValue(attribute, ATTRIBUTE_DEFINE.ATTACK))
            };

            return damage;
        }
        
        /// <summary>
        /// 计算抵抗后的伤害数值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="damage">伤害</param>
        /// <returns>抵抗后的伤害</returns>
        public DamageInfo DischargeDamage(ulong actor, DamageInfo damage)
        {
            if (false == stage.SeekBehaviorInfo(actor, out AttributeInfo attribute)) return damage;

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
            if (false == stage.SeekBehaviorInfo(to, out AttributeInfo toattribute)) return;
            var disdamage = DischargeDamage(toattribute.actor, damage);
            
            var hp = GetAttributeValue(toattribute, ATTRIBUTE_DEFINE.HP);
            var afterhp = Math.Clamp(hp - disdamage.value, 0, GetAttributeValue(toattribute, ATTRIBUTE_DEFINE.MAXHP));
            SetAttributeValue(toattribute, ATTRIBUTE_DEFINE.HP, afterhp);
            
            // 推送伤害事件到渲染层
            var eventdamage = ObjectCache.Ensure<RIL_EVENT_DAMAGE>();
            eventdamage.from = from;
            eventdamage.to = to;
            eventdamage.crit = disdamage.crit;
            eventdamage.damage = disdamage.value;
            stage.rilsync.Send(eventdamage);
            
            if (afterhp > 0) return;
            stage.killer.Kill(from, to);
            
            // TODO 后续要改成真正的死亡流程
            stage.RmvActor(to);
        }
    }
}