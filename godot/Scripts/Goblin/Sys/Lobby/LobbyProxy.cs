using Goblin.Sys.Common;
using Queen.Protocols;

namespace Goblin.Sys.Lobby
{
    public class LobbyProxy : Proxy<LobbyModel>
    {
        public long timestamp { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Recv<S2CHeartbeatMsg>(OnHeartbeat);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnRecv<S2CHeartbeatMsg>(OnHeartbeat);
        }

        private void OnHeartbeat(S2CHeartbeatMsg msg) => timestamp = msg.timestamp;
    }
}
