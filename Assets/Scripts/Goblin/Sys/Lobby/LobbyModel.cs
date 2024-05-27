using Goblin.Sys.Common;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Lobby
{
    /// <summary>
    /// 房间状态
    /// </summary>
    public class RoomState
    {
        /// <summary>
        /// 等待加入
        /// </summary>
        public static readonly int WAITING = 1;
        /// <summary>
        /// 游戏进行
        /// </summary>
        public static readonly int GAMING = 2;
    }

    /// <summary>
    /// 大厅数据
    /// </summary>
    public class LobbyModel : Module<LobbyProxy>
    {
        /// <summary>
        /// 我的房间 ID
        /// </summary>
        public uint myRoom
        {
            get
            {
                foreach (var room in rooms)
                {
                    foreach (var member in room.members)
                    {
                        if (member.pid.Equals(engine.proxy.login.data.pid)) return room.id;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// 加入的房间
        /// </summary>
        public bool hasRoom
        {
            get
            {
                var room = proxy.GetRoom(myRoom);

                return null != room;
            }
        }

        /// <summary>
        /// 拥有者的身份
        /// </summary>
        public bool ownerRoom 
        {
            get
            {
                if (false == hasRoom) return false;
                var room = proxy.GetRoom(myRoom);

                return room.owner.Equals(engine.proxy.login.data.pid);
            }
        }

        /// <summary>
        /// 房间列表
        /// </summary>
        public List<RoomInfo> rooms = new();
    }
}
