﻿using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Common;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Lives;
using Goblin.Gameplay.Logic.Skills.Action;
using Goblin.Gameplay.Logic.Skills.Action.Cache;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.Bullets;
using Goblin.Gameplay.Logic.Skills.Bullets.Common;
using Kowtow.Math;
using MessagePack;
using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能管线结束事件
    /// </summary>
    public struct SkillPipelineStateEvent : IEvent
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public byte state { get; set; }
    }

    /// <summary>
    /// 技能碰撞事件
    /// </summary>
    public struct SkillCollisionEvent : IEvent
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 碰撞的 ActorID 列表
        /// </summary>
        public uint[] actorIds { get; set; }
    }

    /// <summary>
    /// 技能管线
    /// </summary>
    public class SkillPipeline : Comp
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 技能状态
        /// </summary>
        public byte state { get; private set; } = SKILL_PIPELINE_STATE_DEFINE.NONE;
        /// <summary>
        /// 打断标记
        /// </summary>
        public int breaktoken { get; private set; } = BREAK_TOKEN_DEFINE.NONE;
        /// <summary>
        /// 自身跳帧
        /// </summary>
        public uint seflbreakframes { get; set; }
        /// <summary>
        /// 目标跳帧
        /// </summary>
        public uint targetbreakframes { get; set; }
        /// <summary>
        /// 当前执行帧号
        /// </summary>
        public uint frame { get; private set; }
        /// <summary>
        /// 技能帧数
        /// </summary>
        public uint length { get; private set; }
        /// <summary>
        /// 技能释放器
        /// </summary>
        public SkillLauncher launcher { get; set; }
        /// <summary>
        /// 技能行为数据
        /// </summary>
        private List<SkillActionData> actiondatas = new();
        /// <summary>
        /// 技能行为缓存
        /// </summary>
        private Dictionary<SkillActionData, SkillActionCache> cachedict = new();
        /// <summary>
        /// 技能行为
        /// </summary>
        private Dictionary<ushort, SkillAction> actiondict = new();
        /// <summary>
        /// 触发 Buff 数据
        /// </summary>
        private List<BuffTriggerEventData> triggerbuffdatas = new();
        /// <summary>
        /// 印下 Buff 行为数据
        /// </summary>
        private List<BuffStampEventData> stampbuffdatas { get; set; } = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        /// <summary>
        /// 技能释放
        /// </summary>
        /// <returns></returns>
        public bool Launch()
        {
            if (SKILL_PIPELINE_STATE_DEFINE.NONE != state) return false;
            Start();

            return true;
        }

        /// <summary>
        /// 全擦打断标记
        /// </summary>
        public void NoneBreakToken()
        {
            breaktoken = BREAK_TOKEN_DEFINE.NONE;
        }

        /// <summary>
        /// 擦除打断标记
        /// </summary>
        /// <param name="token">打断标记</param>
        public void EraseBreakToken(int token)
        {
            breaktoken &= ~token;
        }

        /// <summary>
        /// 印下打断标记
        /// </summary>
        /// <param name="token"></param>
        public void StampBreakToken(int token)
        {
            breaktoken |= token;
        }
        
        /// <summary>
        /// 清空触发 Buff 数据
        /// </summary>
        public void EmptyTriggerBuffDatas()
        {
            triggerbuffdatas.Clear();
        }
        
        /// <summary>
        /// 移除触发 Buff 数据
        /// </summary>
        /// <param name="data"></param>
        public void RmvTriggerBuffData(BuffTriggerEventData data)
        {
            if (false == triggerbuffdatas.Contains(data)) return;
            triggerbuffdatas.Remove(data);
        }
        
        /// <summary>
        /// 添加触发 Buff 数据
        /// </summary>
        /// <param name="data"></param>
        public void AddTriggerBuffData(BuffTriggerEventData data)
        {
            if (triggerbuffdatas.Contains(data)) return;
            triggerbuffdatas.Add(data);
        }

        /// <summary>
        /// 清空印下 Buff 数据
        /// </summary>
        public void EmptyStampBuffDatas()
        {
            stampbuffdatas.Clear();
        }
        
        /// <summary>
        /// 移除印下 Buff 数据
        /// </summary>
        /// <param name="data"></param>
        public void RmvStampBuffData(BuffStampEventData data)
        {
            if (false == stampbuffdatas.Contains(data)) return;
            stampbuffdatas.Remove(data);
        }
        
        /// <summary>
        /// 添加印下 Buff 数据
        /// </summary>
        /// <param name="data"></param>
        public void AddStampBuffData(BuffStampEventData data)
        {
            if (stampbuffdatas.Contains(data)) return;
            stampbuffdatas.Add(data);
        }

        /// <summary>
        /// 技能打断
        /// </summary>
        public void Break()
        {
            if (SKILL_PIPELINE_STATE_DEFINE.NONE == state) return;
            state = SKILL_PIPELINE_STATE_DEFINE.BREAK;
            NotifyState();
            End();
        }

        /// <summary>
        /// 技能开始
        /// </summary>
        private void Start()
        {
            Reset();
            state = SKILL_PIPELINE_STATE_DEFINE.START;
            NotifyState();
            state = SKILL_PIPELINE_STATE_DEFINE.CASTING;
            NotifyState();
            Casting(GAME_DEFINE.LOGIC_TICK);
        }

        /// <summary>
        /// 技能时间轴驱动
        /// </summary>
        /// <param name="tick"></param>
        private void Casting(FP tick)
        {
            // 执行技能行为
            foreach (var actionData in actiondatas)
            {
                var cache = GetCache(actionData);
                var action = GetAction(actionData.id);
                var nilcache = null == cache;
                var nilaction = null == action;
                switch (actionData.id)
                {
                    case SKILL_ACTION_DEFINE.SPATIAL:
                        if (nilaction) action = AddAction<Spatial>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BULLET_EVENT:
                        if (nilaction) action = AddAction<BulletEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_TOKEN_EVENT:
                        if (nilaction) action = AddAction<BreakTokenEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT:
                        if (nilaction) action = AddAction<BreakFramesEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BUFF_TRIGGER_EVENT:
                        if (nilaction) action = AddAction<BuffTriggerEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BUFF_STAMP_EVENT:
                        if (nilaction) action = AddAction<BuffStampEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BOX_DETECTION:
                        if (nilaction) action = AddAction<BoxDetection>(actionData.id);
                        if (nilcache) cache = GenCache<DetectionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.SPHERE_DETECTION:
                        if (nilaction) action = AddAction<SphereDetection>(actionData.id);
                        if (nilcache) cache = GenCache<DetectionCache>(actionData);
                        break;
                }

                // 进入
                if (frame == actionData.sframe) action.Enter(actionData, cache);
                // 执行
                if (frame >= actionData.sframe && frame <= actionData.eframe) action.Execute(actionData, cache, frame, tick);
                // 离开
                if (frame == actionData.eframe) action.Exit(actionData, cache);
            }

            // 帧号递增
            frame++;
            // 结束
            if (frame >= length) End();
        }

        /// <summary>
        /// 技能结束
        /// </summary>
        private void End()
        {
            state = SKILL_PIPELINE_STATE_DEFINE.END;
            NotifyState();
            Reset();
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            NoneBreakToken();
            EmptyStampBuffDatas();
            seflbreakframes = 0;
            targetbreakframes = 0;
            frame = 0;
            foreach (var cache in cachedict.Values) cache.OnReset();
        }

        /// <summary>
        /// 通知技能状态
        /// </summary>
        private void NotifyState()
        {
            launcher.actor.eventor.Tell(new SkillPipelineStateEvent { id = id, state = state });
        }

        public void OnExecute(FP tick)
        {
            if (SKILL_PIPELINE_STATE_DEFINE.END == state)
            {
                NotifyState();
                state = SKILL_PIPELINE_STATE_DEFINE.NONE;
            }

            if (SKILL_PIPELINE_STATE_DEFINE.CASTING != state) return;

            // Tick
            Casting(tick);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            // 加载技能数据
            var spdata = MessagePackSerializer.Deserialize<SkillPipelineData>(engine.gameres.location.LoadSkillDataSync(id.ToString()));
            // 技能帧数
            length = FP.ToUInt(spdata.length * GAME_DEFINE.LOGIC_SP_DATA_FRAME_SCALE);
            // 解析行为数据
            for (int i = 0; i < spdata.actionIds.Length; i++)
            {
                var actionId = spdata.actionIds[i];
                SkillActionData data = default;
                switch (actionId)
                {
                    case SKILL_ACTION_DEFINE.SPATIAL:
                        data = MessagePackSerializer.Deserialize<SpatialData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BULLET_EVENT:
                        data = MessagePackSerializer.Deserialize<BulletEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_TOKEN_EVENT:
                        data = MessagePackSerializer.Deserialize<BreakTokenEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT:
                        data = MessagePackSerializer.Deserialize<BreakFramesEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BUFF_TRIGGER_EVENT:
                        data = MessagePackSerializer.Deserialize<BuffTriggerEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BUFF_STAMP_EVENT:
                        data = MessagePackSerializer.Deserialize<BuffStampEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BOX_DETECTION:
                        data = MessagePackSerializer.Deserialize<BoxDetectionData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.SPHERE_DETECTION:
                        data = MessagePackSerializer.Deserialize<SphereDetectionData>(spdata.actionBytes[i]);
                        break;
                }

                if (null == data) continue;
                data.sframe = FP.ToUInt(data.sframe * GAME_DEFINE.LOGIC_SP_DATA_FRAME_SCALE);
                data.eframe = FP.ToUInt(data.eframe * GAME_DEFINE.LOGIC_SP_DATA_FRAME_SCALE);
                actiondatas.Add(data);
            }
        }

        /// <summary>
        /// 获取技能行为
        /// </summary>
        /// <param name="id">技能行为 ID</param>
        /// <returns>技能行为</returns>
        private SkillAction GetAction(ushort id)
        {
            if (null == actiondict) return default;

            return actiondict.GetValueOrDefault(id);
        }

        /// <summary>
        /// 获取技能行为缓存
        /// </summary>
        /// <param name="data">技能行为数据</param>
        /// <returns>技能行为缓存</returns>
        public SkillActionCache GetCache(SkillActionData data)
        {
            var cache = cachedict.GetValueOrDefault(data);

            return cache;
        }

        /// <summary>
        /// 生成技能行为缓存
        /// </summary>
        /// <param name="data">技能行为数据</param>
        /// <typeparam name="TC">技能行为缓存类型</typeparam>
        /// <returns>技能行为缓存</returns>
        public TC GenCache<TC>(SkillActionData data) where TC : SkillActionCache, new()
        {
            var cache = new TC();
            cachedict.Add(data, cache);

            return cache;
        }

        /// <summary>
        /// 添加技能行为
        /// </summary>
        /// <param name="id">技能行为 ID</param>
        /// <typeparam name="T">技能行为类型</typeparam>
        /// <returns>技能行为</returns>
        /// <exception cref="Exception">无法添加相同的行为</exception>
        private SkillAction AddAction<T>(ushort id) where T : SkillAction, new()
        {
            if (null == actiondict) actiondict = new();
            if (actiondict.ContainsKey(id)) throw new Exception($"can't add same skillaction -> {typeof(T)}");

            var action = AddComp<T>();
            action.pipeline = this;
            action.Create();
            actiondict.Add(id, action);

            return action;
        }

        /// <summary>
        /// 技能命中
        /// </summary>
        /// <param name="actorIds">ActorID 集合</param>
        public void OnHit(uint[] actorIds)
        {
            if (null == actorIds || 0 == actorIds.Length) return;

            BreakFramesByHit(actorIds);
            BuffByHit(actorIds);

            launcher.actor.eventor.Tell(new SkillCollisionEvent { id = id, actorIds = actorIds });
        }

        /// <summary>
        /// 受击顿帧
        /// </summary>
        /// <param name="actorIds">ActorID 集合</param>
        private void BreakFramesByHit(uint[] actorIds)
        {
            launcher.actor.ticker.breakframes += seflbreakframes;
            foreach (uint actorId in actorIds)
            {
                var target = launcher.actor.stage.GetActor(actorId);
                if (null == target || false == target.live.alive) continue;
                target.ticker.breakframes += targetbreakframes;
            }
        }

        /// <summary>
        /// 处理 Buff
        /// </summary>
        /// <param name="actorIds">ActorID 集合</param>
        private void BuffByHit(uint[] actorIds)
        {
            foreach (var data in triggerbuffdatas)
            {
                if (BUFF_DEFINE.ACTIVE_SELF_HIT == (data.triggerself & BUFF_DEFINE.ACTIVE_SELF_HIT))
                {
                    launcher.actor.eventor.Tell(new Buffs.Common.BuffTriggerEvent
                    {
                        id = data.buffid,
                        from = launcher.actor.id,
                    });
                }
                
                if (BUFF_DEFINE.ACTIVE_TARGET_HIT != (data.triggertarget & BUFF_DEFINE.ACTIVE_TARGET_HIT)) continue;
                
                foreach (uint actorId in actorIds)
                {
                    var target = launcher.actor.stage.GetActor(actorId);
                    if (null == target) continue;
                    target.eventor.Tell(new Buffs.Common.BuffTriggerEvent
                    {
                        id = data.buffid,
                        from = launcher.actor.id,
                    });
                }
            }            
            
            foreach (var data in stampbuffdatas)
            {
                if (BUFF_DEFINE.ACTIVE_SELF_HIT == (data.stampself & BUFF_DEFINE.ACTIVE_SELF_HIT))
                {
                    launcher.actor.eventor.Tell(new Buffs.Common.BuffStampEvent
                    {
                        id = data.buffid,
                        layer = data.layer,
                        from = launcher.actor.id,
                    });
                }

                if (BUFF_DEFINE.ACTIVE_TARGET_HIT != (data.stamptarget & BUFF_DEFINE.ACTIVE_TARGET_HIT)) continue;
                
                foreach (uint actorId in actorIds)
                {
                    var target = launcher.actor.stage.GetActor(actorId);
                    if (null == target) continue;
                    target.eventor.Tell(new Buffs.Common.BuffStampEvent
                    {
                        id = data.buffid,
                        layer = data.layer,
                        from = launcher.actor.id,
                    });
                }
            }
        }
    }
}
