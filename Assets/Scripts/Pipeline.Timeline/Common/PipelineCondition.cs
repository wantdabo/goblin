using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pipeline.Timeline.Common
{
    /// <summary>
    /// 管线条件包装
    /// </summary>
    [Serializable]
    public class PipelineCondition
    {
        [HideLabel]
        [SerializeReference, InlineProperty]
        [TypeFilter("GetFilteredTypes")]
        public Condition condition;
        
        private static IEnumerable<Type> GetFilteredTypes()
        {
            return typeof(Condition).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(Condition).IsAssignableFrom(t));
        }
    }
}