using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Render.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// Buff 桶
    /// </summary>
    public class BuffBucket : Behavior
    {
        private Dictionary<uint, (uint id, byte type, byte state, uint layer, uint maxlayer, uint from)> buffdict { get; set; } = new();
    }
}
