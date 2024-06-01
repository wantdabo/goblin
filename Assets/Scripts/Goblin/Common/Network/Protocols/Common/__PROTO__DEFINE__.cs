using System;
using System.Collections.Generic;

namespace Queen.Protocols.Common
{
    public partial class ProtoPack
    {
        /// <summary>
        /// 协议号定义
        /// </summary>
        private static Dictionary<uint, Type> messageIds = new()
        {
            { 10001, typeof(Queen.Protocols.S2C_GameInfoMsg)},
            { 10002, typeof(Queen.Protocols.C2SLogoutMsg)},
            { 10003, typeof(Queen.Protocols.C2SLoginMsg)},
            { 10004, typeof(Queen.Protocols.C2SRegisterMsg)},
            { 10005, typeof(Queen.Protocols.S2CLogoutMsg)},
            { 10006, typeof(Queen.Protocols.S2CLoginMsg)},
            { 10007, typeof(Queen.Protocols.S2CRegisterMsg)},
            { 10008, typeof(Queen.Protocols.C2S_ExitRoomMsg)},
            { 10009, typeof(Queen.Protocols.C2S_KickRoomMsg)},
            { 10010, typeof(Queen.Protocols.C2S_JoinRoomMsg)},
            { 10011, typeof(Queen.Protocols.C2S_Room2GameMsg)},
            { 10012, typeof(Queen.Protocols.C2S_DestroyRoomMsg)},
            { 10013, typeof(Queen.Protocols.C2S_CreateRoomMsg)},
            { 10014, typeof(Queen.Protocols.C2S_PullRoomsMsg)},
            { 10015, typeof(Queen.Protocols.S2C_ExitRoomMsg)},
            { 10016, typeof(Queen.Protocols.S2C_KickRoomMsg)},
            { 10017, typeof(Queen.Protocols.S2C_JoinRoomMsg)},
            { 10018, typeof(Queen.Protocols.S2C_Room2GameMsg)},
            { 10019, typeof(Queen.Protocols.S2C_DestroyRoomMsg)},
            { 10020, typeof(Queen.Protocols.S2C_CreateRoomMsg)},
            { 10021, typeof(Queen.Protocols.S2C_PushRoomsMsg)},
            { 10022, typeof(Queen.Protocols.S2C_PushRoomMsg)},
            { 10023, typeof(Queen.Protocols.S2G_CreateStageMsg)},
            { 10024, typeof(Queen.Protocols.G2S_CreateStageMsg)},
            { 10025, typeof(Queen.Protocols.S2G_DestroyStageMsg)},
            { 10026, typeof(Queen.Protocols.G2S_DestroyStageMsg)},
            { 10027, typeof(Queen.Protocols.C2G_StartStageMsg)},
            { 10028, typeof(Queen.Protocols.G2C_StartStageMsg)},
            { 10029, typeof(Queen.Protocols.C2G_SetInputMsg)},
            { 10030, typeof(Queen.Protocols.G2C_LogicTickMsg)},
            { 10031, typeof(Queen.Protocols.Common.NodePingMsg)},
        };
    }
}

