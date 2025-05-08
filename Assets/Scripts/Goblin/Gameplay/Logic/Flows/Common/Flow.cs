using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管线流
    /// </summary>
    public class Flow : Behavior
    {
        private Dictionary<ushort, Checker> checkers { get; set; }
        private Dictionary<ushort, Executor> executors { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            Checkers();
            Executors();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            foreach (var kv in checkers)
            {
                kv.Value.Unload();
                ObjectCache.Set(kv.Value);
            }
            checkers.Clear();
            ObjectCache.Set(checkers);
            
            foreach (var kv in executors)
            {
                kv.Value.Unload();
                ObjectCache.Set(kv.Value);
            }
            executors.Clear();
            ObjectCache.Set(executors);
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos<PipelineInfo>(out var pipelineinfos)) return;
            foreach (var pipelineinfo in pipelineinfos)
            {
                foreach (var pipelineid in pipelineinfo.pipelines)
                {
                    // TODO 临时虚构一个, 后续要根据 ID 进行加载管线数据
                    PipelineData data = null;
                    if (false == pipelineinfo.doings.TryGetValue(pipelineid, out var indexes)) pipelineinfo.doings.Add(pipelineid, indexes = ObjectCache.Get<List<uint>>());
                    if (false == data.Query(pipelineinfo.timeline, out var instrinfos)) continue;

                    foreach ((bool inside, uint index, Instruct instruct) instrinfo in instrinfos)
                    {
                        if (false == instrinfo.inside)
                        {
                            if (indexes.Contains(instrinfo.index))
                            {
                                
                            }

                            continue;
                        }

                        bool pass = true;
                        foreach (var condition in instrinfo.instruct.conditions)
                        {
                            if (false == checkers.TryGetValue(condition.id, out var checker)) throw new Exception($"id : {condition.id} cannot find checker.");
                            if (checker.Check(condition, pipelineinfo)) continue;
                            pass = false;
                            break;
                        }

                        if (false == pass)
                        {
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化指令条件检查器
        /// </summary>
        private void Checkers()
        {
            checkers = ObjectCache.Get<Dictionary<ushort, Checker>>();
        }

        /// <summary>
        /// 初始化指令执行器
        /// </summary>
        private void Executors()
        {
            executors = ObjectCache.Get<Dictionary<ushort, Executor>>();
        }
    }
}