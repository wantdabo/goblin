using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Goblin.Sys.Other.View;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Lobby
{
    /// <summary>
    /// 房间创建事件
    /// </summary>
    public struct RoomCreatedEvent : IEvent
    {
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id;
    }

    /// <summary>
    /// 房间销毁事件
    /// </summary>
    public struct RoomDestroyedEvent : IEvent
    {
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id;
    }

    /// <summary>
    /// 房间列表变化事件
    /// </summary>
    public struct RoomsChangedEvent : IEvent { }

    /// <summary>
    /// 房间信息变化事件
    /// </summary>
    public struct RoomChangedEvent : IEvent
    {
        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id;
    }

    /// <summary>
    /// 大厅
    /// </summary>
    public class LobbyProxy : Proxy<LobbyModel>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
