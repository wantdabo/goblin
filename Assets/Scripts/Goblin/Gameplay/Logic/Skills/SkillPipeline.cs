using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Logic.Skills.Action;
using Goblin.Gameplay.Logic.Skills.Action.Common;
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
        public byte state { get; private set; } = SkillPipelineStateDef.None;
        /// <summary>
        /// 打断标记
        /// </summary>
        public int breaktoken { get; private set; } = BreakTokenDef.NONE;
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
        private List<SkillActionData> actionDatas = new();
        /// <summary>
        /// 技能行为
        /// </summary>
        private Dictionary<ushort, SkillAction> actionDict = new();

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
            if (SkillPipelineStateDef.None != state) return false;
            Start();

            return true;
        }

        /// <summary>
        /// 全擦打断标记
        /// </summary>
        public void NoneBreakToken()
        {
            breaktoken = BreakTokenDef.NONE;
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
            if (SkillPipelineStateDef.None == state) return;
            state = SkillPipelineStateDef.Break;
            NotifyState();
            End();
        }

        /// <summary>
        /// 技能开始
        /// </summary>
        private void Start()
        {
            state = SkillPipelineStateDef.Start;
            NoneBreakToken();
            seflbreakframes = 0;
            targetbreakframes = 0;
            NotifyState();
            frame = 0;
            state = SkillPipelineStateDef.Casting;
            NotifyState();
            Casting(GameDef.LOGIC_TICK);
        }

        /// <summary>
        /// 技能时间轴驱动
        /// </summary>
        /// <param name="tick"></param>
        private void Casting(FP tick)
        {
            // 执行技能行为
            foreach (var actionData in actionDatas)
            {
                var action = GetAction(actionData.id);
                if (null == action)
                {
                    switch (actionData.id)
                    {
                        case SkillActionDef.SPATIAL:
                            action = AddAction<SpatialAction>(actionData.id);
                            break;
                        case SkillActionDef.SKILL_BREAK_EVENT:
                            action = AddAction<SkillBreakEventAction>(actionData.id);
                            break;
                        case SkillActionDef.SKILL_BREAK_FRAMES_EVENT:
                            action = AddAction<SkillBreakFramesAction>(actionData.id);
                            break;
                        case SkillActionDef.BOX_DETECTION:
                            action = AddAction<BoxDetectionAction>(actionData.id);
                            break;
                        case SkillActionDef.SPHERE_DETECTION:
                            action = AddAction<SphereDetectionAction>(actionData.id);
                            break;
                        case SkillActionDef.CYLINDER_DETECTION:
                            action = AddAction<CylinderDetectionAction>(actionData.id);
                            break;
                    }
                }

                // 进入
                if (frame == actionData.sframe) action.Enter(actionData);
                // 执行
                if (frame >= actionData.sframe && frame <= actionData.eframe) action.Execute(actionData, frame, tick);
                // 离开
                if (frame == actionData.eframe) action.Exit(actionData);
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
            state = SkillPipelineStateDef.End;
            NotifyState();
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
            if (SkillPipelineStateDef.End == state)
            {
                NotifyState();
                state = SkillPipelineStateDef.None;
            }

            if (SkillPipelineStateDef.Casting != state) return;

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
            length = FP.ToUInt(spdata.length * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
            // 解析行为数据
            for (int i = 0; i < spdata.actionIds.Length; i++)
            {
                var actionId = spdata.actionIds[i];
                SkillActionData data = default;
                switch (actionId)
                {
                    case SkillActionDef.SPATIAL:
                        data = MessagePackSerializer.Deserialize<SpatialActionData>(spdata.actionBytes[i]);
                        break;
                    case SkillActionDef.SKILL_BREAK_EVENT:
                        data = MessagePackSerializer.Deserialize<SkillBreakEventActionData>(spdata.actionBytes[i]);
                        break;
                    case SkillActionDef.SKILL_BREAK_FRAMES_EVENT:
                        data = MessagePackSerializer.Deserialize<SkillBreakFramesActionData>(spdata.actionBytes[i]);
                        break;
                    case SkillActionDef.BOX_DETECTION:
                        data = MessagePackSerializer.Deserialize<BoxDetectionActionData>(spdata.actionBytes[i]);
                        break;
                    case SkillActionDef.SPHERE_DETECTION:
                        data = MessagePackSerializer.Deserialize<SphereDetectionActionData>(spdata.actionBytes[i]);
                        break;
                    case SkillActionDef.CYLINDER_DETECTION:
                        data = MessagePackSerializer.Deserialize<CylinderDetectionActionData>(spdata.actionBytes[i]);
                        break;
                }

                if (null == data) continue;
                data.sframe = FP.ToUInt(data.sframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                data.eframe = FP.ToUInt(data.eframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                actionDatas.Add(data);
            }
        }

        /// <summary>
        /// 获取技能行为
        /// </summary>
        /// <param name="id">技能行为 ID</param>
        /// <returns>技能行为</returns>
        private SkillAction GetAction(ushort id)
        {
            if (null == actionDict) return default;

            return actionDict.GetValueOrDefault(id);
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
            if (null == actionDict) actionDict = new();
            if (actionDict.ContainsKey(id)) throw new Exception($"can't add same skillaction -> {typeof(T)}");

            var action = AddComp<T>();
            action.pipeline = this;
            action.Create();
            actionDict.Add(id, action);

            return action;
        }
        
        /// <summary>
        /// 技能命中
        /// </summary>
        /// <param name="actorIds"></param>
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
