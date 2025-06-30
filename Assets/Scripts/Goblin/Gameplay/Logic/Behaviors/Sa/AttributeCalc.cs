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
            var value = attribute.datas.GetValueOrDefault(attrkey.mainkey, 0);
            var scale = attribute.datas.GetValueOrDefault(attrkey.scalekey, 1000);
            
            return Math.Clamp((value * (scale * stage.cfg.int2fp)).AsInt(), 0, int.MaxValue);
        }
        
        /// <summary>
        /// 获取属性的千分比值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <returns>数值</returns>
        public int GetAttributeScaleValue(AttributeInfo attribute, ushort key)
        {
            var attrkey = ConvAttribyteKey(key);
            
            return attribute.datas.GetValueOrDefault(attrkey.scalekey, 1000);
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
            if (attribute.datas.ContainsKey(attrkey.mainkey)) attribute.datas.Remove(attrkey.mainkey);
            attribute.datas.Add(attrkey.mainkey, value);
        }

        /// <summary>
        /// 设置属性的千分比值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <param name="value">数值</param>
        /// <exception cref="Exception">HP 属性的千分比值不允许被修改</exception>
        public void SetAttributeScaleValue(AttributeInfo attribute, ushort key, int value)
        {
            if (ATTRIBUTE_DEFINE.HP == key) throw new Exception("HP 属性的千分比值不允许被修改");
            
            var attrkey = ConvAttribyteKey(key);
            if (attribute.datas.ContainsKey(attrkey.scalekey)) attribute.datas.Remove(attrkey.scalekey);
            attribute.datas.Add(attrkey.scalekey, value);
        }
        
        /// <summary>
        /// 修改属性值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <param name="value">数值</param>
        /// <param name="clamp">约束范围</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public (int before, int after) ChangeAttributeValue(AttributeInfo attribute, ushort key, int value, bool clamp = false, int min = 0, int max = 0)
        {
            var before = GetAttributeValue(attribute, key);
            var changevalue = before + value;
            if (clamp) changevalue = Math.Clamp(changevalue, min, max);
            SetAttributeValue(attribute, key, changevalue);
            var after = GetAttributeValue(attribute, key);
            
            return (before, after);
        }
        
        /// <summary>
        /// 修改属性的千分比值
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="key">属性 Key</param>
        /// <param name="value">数值</param>
        /// <param name="clamp">约束范围</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>结果</returns>
        public (int before, int after) ChangeAttributeScaleValue(AttributeInfo attribute, ushort key, int value, bool clamp = false, int min = 0, int max = 0)
        {
            var before = GetAttributeScaleValue(attribute, key);
            var changevalue = before + value;
            if (clamp) changevalue = Math.Clamp(changevalue, min, max);
            SetAttributeScaleValue(attribute, key, changevalue);
            var after = GetAttributeScaleValue(attribute, key);
            
            return (before, after);
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
            var result = ChangeAttributeValue(toattribute, ATTRIBUTE_DEFINE.HP, -disdamage.value, true, 0, GetAttributeValue(toattribute, ATTRIBUTE_DEFINE.MAXHP));
            
            // TODO 后续要改为统一推送这类事件到渲染层 (伤害跳字, 治疗跳字, 效果跳字等)
            // 推送伤害事件到渲染层
            var eventdamage = ObjectCache.Ensure<RIL_EVENT_DAMAGE>();
            eventdamage.from = from;
            eventdamage.to = to;
            eventdamage.crit = disdamage.crit;
            eventdamage.damage = result.before - result.after;
            stage.rilsync.Send(eventdamage);
            
            if (result.after > 0) return;
            stage.silentmercy.Kill(from, to);
            
            // TODO 后续要改成真正的死亡流程
            stage.RmvActor(to);
        }
    }
}