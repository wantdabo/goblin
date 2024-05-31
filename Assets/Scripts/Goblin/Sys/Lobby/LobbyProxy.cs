using Goblin.Common;
using Goblin.Gameplay.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
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
            engine.net.Recv<S2C_ExitRoomMsg>(OnS2CExitRoom);
            engine.net.Recv<S2C_KickRoomMsg>(OnS2CKickRoom);
            engine.net.Recv<S2C_JoinRoomMsg>(OnS2CJoinRoom);
            engine.net.Recv<S2C_Room2GameMsg>(OnS2CRoom2Game);
            engine.net.Recv<S2C_DestroyRoomMsg>(OnS2CDestroyRoom);
            engine.net.Recv<S2C_CreateRoomMsg>(OnS2CCreateRoom);
            engine.net.Recv<S2C_PushRoomsMsg>(OnS2CPushRooms);
            engine.net.Recv<S2C_PushRoomMsg>(OnS2CPushRoom);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnRecv<S2C_ExitRoomMsg>(OnS2CExitRoom);
            engine.net.UnRecv<S2C_KickRoomMsg>(OnS2CKickRoom);
            engine.net.UnRecv<S2C_JoinRoomMsg>(OnS2CJoinRoom);
            engine.net.UnRecv<S2C_DestroyRoomMsg>(OnS2CDestroyRoom);
            engine.net.UnRecv<S2C_CreateRoomMsg>(OnS2CCreateRoom);
            engine.net.UnRecv<S2C_PushRoomsMsg>(OnS2CPushRooms);
            engine.net.UnRecv<S2C_PushRoomMsg>(OnS2CPushRoom);
            engine.net.UnRecv<S2C_PushRoomsMsg>(OnS2CPushRooms);
        }

        /// <summary>
        /// 请求退出房间
        /// </summary>
        public void C2SExitRoom()
        {
            engine.net.Send(new C2S_ExitRoomMsg { });
        }

        /// <summary>
        /// 请求踢出房间
        /// </summary>
        /// <param name="pid">玩家 ID</param>
        public void C2SKickRoom(string pid)
        {
            engine.net.Send(new C2S_KickRoomMsg { pid = pid });
        }

        /// <summary>
        /// 请求加入房间
        /// </summary>
        /// <param name="id">房间 ID</param>
        /// <param name="password">密码</param>
        public void C2SJoinRoom(uint id, uint password)
        {
            engine.net.Send(new C2S_JoinRoomMsg { id = id, password = password });
        }

        /// <summary>
        /// 请求房间开局
        /// </summary>
        public void C2SRoom2Game()
        {
            engine.net.Send(new C2S_Room2GameMsg { });
        }

        /// <summary>
        /// 请求销毁房间
        /// </summary>
        public void C2SDestroyRoom()
        {
            engine.net.Send(new C2S_DestroyRoomMsg { });
        }

        /// <summary>
        /// 请求所有房间列表
        /// </summary>
        public void C2SPullRooms()
        {
            engine.net.Send(new C2S_PullRoomsMsg { });
        }

        /// <summary>
        /// 请求创建房间
        /// </summary>
        /// <param name="name">房间名</param>
        /// <param name="needpwd">需要密码</param>
        /// <param name="password">密码</param>
        /// <param name="mlimit">人数限制</param>
        public void C2SCreateRoom(string name, bool needpwd, uint password, int mlimit)
        {
            engine.net.Send(new C2S_CreateRoomMsg { name = name, needpwd = needpwd, password = password, mlimit = mlimit });
        }

        private void OnS2CExitRoom(S2C_ExitRoomMsg msg)
        {
            if (1 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "您已退出房间." });
                engine.gameui.Close<LobbyRoomView>();
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "未进入任何房间." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "无法退出，游戏进行中." });
            }
        }

        private void OnS2CKickRoom(S2C_KickRoomMsg msg)
        {
            if (1 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "该成员已被请离房间." });
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "您已被请离房间." });
                engine.gameui.Close<LobbyRoomView>();
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "该成员不存在此房间." });
            }
            else if (4 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "您没有该权限这么做." });
            }
        }

        private void OnS2CJoinRoom(S2C_JoinRoomMsg msg)
        {
            if (1 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "成功加入房间." });
                engine.gameui.Close<LobbyRoomNewView>();
                engine.gameui.Close<LobbyRoomJoinView>();
                engine.gameui.Open<LobbyRoomView>();
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间密码错误." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间不存在." });
            }
            else if (4 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "请先退出当前房间." });
            }
            else if (5 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间成员已满." });
            }
            else if (6 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间已经在对局中." });
            }
        }

        private void OnS2CRoom2Game(S2C_Room2GameMsg msg)
        {
            if (1 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "房间开局成功." });
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间已经在对局中." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间不存在." });
            }
            else if (4 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "无法开启房间." });
            }
            else if (5 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "您没有权限这么做." });
            }
        }

        private void OnS2CDestroyRoom(S2C_DestroyRoomMsg msg)
        {
            if (1 == msg.code)
            {
                var room = data.rooms.FirstOrDefault(r => r.id == msg.id);
                if (null != room)
                {
                    eventor.Tell(new RoomDestroyedEvent { id = msg.id });
                    if (engine.proxy.lobby.data.myRoom == room.id)
                    {
                        engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间已被销毁." });
                        engine.gameui.Close<GameplayView>();
                        engine.gameui.Close<LobbyRoomView>();
                        engine.gameui.Open<LobbyView>();
                    }
                    data.rooms.Remove(room);
                    eventor.Tell<RoomsChangedEvent>();
                }
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "您没有权限这么做." });
            }
            else if (3 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "房间不存在." });
            }
        }

        private void OnS2CCreateRoom(S2C_CreateRoomMsg msg)
        {
            if (1 == msg.code)
            {
                eventor.Tell(new RoomCreatedEvent { id = msg.id });
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "创建房间成功." });
            }
            else if (2 == msg.code)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "请先退出当前房间." });
            }
        }

        private void OnS2CPushRooms(S2C_PushRoomsMsg msg)
        {
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "获取到一份最新的房间列表." });
            data.rooms = msg.rooms.ToList();
            eventor.Tell<RoomsChangedEvent>();
        }

        private void OnS2CPushRoom(S2C_PushRoomMsg msg)
        {
            var r = data.rooms.FirstOrDefault(r => r.id == msg.room.id);
            if (null != r) data.rooms.Remove(r);
            data.rooms.Add(msg.room);

            if (null == r) eventor.Tell<RoomsChangedEvent>(); else eventor.Tell(new RoomChangedEvent { id = r.id });
        }

        public RoomInfo GetRoom(uint id)
        {
            return data.rooms.FirstOrDefault(r => r.id == id);
        }
    }
}
