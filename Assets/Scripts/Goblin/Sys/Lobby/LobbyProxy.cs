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
    /// 大厅
    /// </summary>
    public class LobbyProxy : Proxy<LobbyModel>
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Recv<S2CHeartbeatMsg>(OnHeartbeat);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnHeartbeat(S2CHeartbeatMsg msg)
        {
            timestamp = msg.timestamp;
        }
    }
}
