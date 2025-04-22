using System.Collections.Generic;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 标签状态
    /// </summary>
    public struct TagState : IState
    {
        public StateType type => StateType.Tag;
        public ulong actor { get; set; }
        public uint frame { get; set; }
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }
    }
}