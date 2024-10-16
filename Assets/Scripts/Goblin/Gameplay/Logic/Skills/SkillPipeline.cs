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
        public byte state { get; private set; } = SPStateDef.None;
        /// <summary>
        /// 技能发射器
        /// </summary>
        public SkillLauncher launcher { get; set; }
        /// <summary>
        /// 当前执行帧号
        /// </summary>
        public uint frame { get; private set; }
        /// <summary>
        /// 帧数
        /// </summary>
        public uint length { get; private set; }
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

        public void Launch()
        {
            if (SPStateDef.None != state) return;
            Start();
        }
        
        public void Break()
        {
            if (SPStateDef.None == state) return;
            state = SPStateDef.Break;
            NotifyState();
            End();
        }

        private void Start()
        {
            state = SPStateDef.Start;
            NotifyState();
            frame = 0;
            state = SPStateDef.Casting;
            NotifyState();
        }

        private void Casting(FP tick)
        {
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
                    }
                }

                if (frame >= actionData.sframe && frame <= actionData.eframe) action.Execute(actionData, frame, tick);
            }

            frame++;
            // 结束
            if (frame >= length) End();
        }

        private void End()
        {
            state = SPStateDef.End;
            NotifyState();
        }

        private void NotifyState()
        {
            launcher.actor.eventor.Tell(new SkillPipelineStateEvent { id = id, state = state});
        }

        public void OnFPTick(FP tick)
        {
            if (SPStateDef.End == state)
            {
                NotifyState();
                state = SPStateDef.None;
            }
            if (SPStateDef.Casting != state) return;
            Casting(tick);
        }

        private void Initialize()
        {
            var spdata = MessagePackSerializer.Deserialize<SkillPipelineData>(engine.gameres.location.LoadSkillDataSync(id.ToString()));
            length = (uint)(spdata.length * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
            for (int i = 0; i < spdata.actionIds.Length; i++)
            {
                var actionId = spdata.actionIds[i];
                switch (actionId)
                {
                    case SkillActionDef.SPATIAL:
                        var spatialData = MessagePackSerializer.Deserialize<SpatialActionData>(spdata.actionBytes[i]);
                        spatialData.sframe = (uint)(spatialData.sframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                        spatialData.eframe = (uint)(spatialData.eframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                        actionDatas.Add(spatialData);
                        break;
                    case SkillActionDef.SKILL_BREAK_EVENT:
                        var skillBreakeventData = MessagePackSerializer.Deserialize<SkillBreakEventActionData>(spdata.actionBytes[i]);
                        skillBreakeventData.sframe = (uint)(skillBreakeventData.sframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                        skillBreakeventData.eframe = (uint)(skillBreakeventData.eframe * GameDef.LOGIC_SP_DATA_FRAME_SCALE);
                        actionDatas.Add(skillBreakeventData);
                        break;
                }
            }
        }

        private SkillAction GetAction(ushort id)
        {
            if (null == actionDict) return default;

            return actionDict.GetValueOrDefault(id);
        }

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
    }
}
