using System;
using System.Collections.Generic;

namespace Queen.Protocols.Common
{
    public partial class ProtoPack
    {
        /// <summary>
        /// 协议号定义
        /// </summary>
        private static List<Type> messageIds = new()
        {
            typeof(Queen.Protocols.S2C_GameInfoMsg),
            typeof(Queen.Protocols.C2SLogoutMsg),
            typeof(Queen.Protocols.C2SLoginMsg),
            typeof(Queen.Protocols.C2SRegisterMsg),
            typeof(Queen.Protocols.S2CLogoutMsg),
            typeof(Queen.Protocols.S2CLoginMsg),
            typeof(Queen.Protocols.S2CRegisterMsg),
            typeof(Queen.Protocols.C2S_ExitRoomMsg),
            typeof(Queen.Protocols.C2S_KickRoomMsg),
            typeof(Queen.Protocols.C2S_JoinRoomMsg),
            typeof(Queen.Protocols.C2S_Room2GameMsg),
            typeof(Queen.Protocols.C2S_DestroyRoomMsg),
            typeof(Queen.Protocols.C2S_CreateRoomMsg),
            typeof(Queen.Protocols.C2S_PullRoomsMsg),
            typeof(Queen.Protocols.S2C_ExitRoomMsg),
            typeof(Queen.Protocols.S2C_KickRoomMsg),
            typeof(Queen.Protocols.S2C_JoinRoomMsg),
            typeof(Queen.Protocols.S2C_Room2GameMsg),
            typeof(Queen.Protocols.S2C_DestroyRoomMsg),
            typeof(Queen.Protocols.S2C_CreateRoomMsg),
            typeof(Queen.Protocols.S2C_PushRoomsMsg),
            typeof(Queen.Protocols.S2C_PushRoomMsg),
            typeof(Queen.Protocols.S2G_CreateStageMsg),
            typeof(Queen.Protocols.G2S_CreateStageMsg),
            typeof(Queen.Protocols.S2G_DestroyStageMsg),
            typeof(Queen.Protocols.G2S_DestroyStageMsg),
            typeof(Queen.Protocols.C2G_StartStageMsg),
            typeof(Queen.Protocols.G2C_StartStageMsg),
            typeof(Queen.Protocols.C2G_SetInputMsg),
            typeof(Queen.Protocols.G2C_LogicTickMsg),
            typeof(Queen.Protocols.Common.NodePingMsg),
        };
    }
}

