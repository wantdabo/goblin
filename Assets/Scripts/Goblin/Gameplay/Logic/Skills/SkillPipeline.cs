using Goblin.Common;
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
using MessagePack;
using System;
using System.Collections.Generic;
using TrueSync;

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
        public byte state { get; private set; } = SKILL_PIPELINE_STATE_DEFINE.None;
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
            if (SKILL_PIPELINE_STATE_DEFINE.None != state) return false;
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
        /// 技能打断
        /// </summary>
        public void Break()
        {
            if (SKILL_PIPELINE_STATE_DEFINE.None == state) return;
            state = SKILL_PIPELINE_STATE_DEFINE.Break;
            NotifyState();
            End();
        }

        /// <summary>
        /// 技能开始
        /// </summary>
        private void Start()
        {
            Reset();
            state = SKILL_PIPELINE_STATE_DEFINE.Start;
            NotifyState();
            state = SKILL_PIPELINE_STATE_DEFINE.Casting;
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
                    case SKILL_ACTION_DEFINE.BREAK_EVENT:
                        if (nilaction) action = AddAction<BreakEvent>(actionData.id);
                        if (nilcache) cache = GenCache<SkillActionCache>(actionData);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT:
                        if (nilaction) action = AddAction<BreakFrames>(actionData.id);
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
                    case SKILL_ACTION_DEFINE.CYLINDER_DETECTION:
                        if (nilaction) action = AddAction<CylinderDetection>(actionData.id);
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
            state = SKILL_PIPELINE_STATE_DEFINE.End;
            NotifyState();
            Reset();
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            NoneBreakToken();
            seflbreakframes = 0;
            targetbreakframes = 0;
            frame = 0;
            foreach (var kv in cachedict) kv.Value.OnReset();
        }

        /// <summary>
        /// 通知技能状态
        /// </summary>
        private void NotifyState()
        {
            launcher.actor.eventor.Tell(new SkillPipelineStateEvent { id = id, state = state });
        }

        public void OnFPTick(FP tick)
        {
            if (SKILL_PIPELINE_STATE_DEFINE.End == state)
            {
                NotifyState();
                state = SKILL_PIPELINE_STATE_DEFINE.None;
            }

            if (SKILL_PIPELINE_STATE_DEFINE.Casting != state) return;

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
                    case SKILL_ACTION_DEFINE.BREAK_EVENT:
                        data = MessagePackSerializer.Deserialize<BreakEventData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT:
                        data = MessagePackSerializer.Deserialize<BreakFramesData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.BOX_DETECTION:
                        data = MessagePackSerializer.Deserialize<BoxDetectionData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.SPHERE_DETECTION:
                        data = MessagePackSerializer.Deserialize<SphereDetectionData>(spdata.actionBytes[i]);
                        break;
                    case SKILL_ACTION_DEFINE.CYLINDER_DETECTION:
                        data = MessagePackSerializer.Deserialize<CylinderDetectionData>(spdata.actionBytes[i]);
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

            // 顿帧
            launcher.actor.ticker.breakframes += seflbreakframes;
            foreach (uint id in actorIds)
            {
                var target = launcher.actor.stage.GetActor(id);
                if (null == target) continue;
                target.ticker.breakframes += targetbreakframes;
            }

            launcher.actor.eventor.Tell(new SkillCollisionEvent { id = id, actorIds = actorIds });
        }
    }
}
