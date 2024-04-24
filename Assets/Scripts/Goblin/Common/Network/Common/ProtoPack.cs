using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Network.Protocols.Common
{
    /// <summary>
    /// 消息结构接口
    /// </summary>
    public interface INetMessage { }

    /// <summary>
    /// 协议序列化
    /// </summary>
    public class ProtoPack
    {
        /// <summary>
        /// 协议号长度
        /// </summary>
        private static int INT32_LEN = 4;

        /// <summary>
        /// 反序列化消息
        /// </summary>
        /// <param name="bytes">二进制数据</param>
        /// <param name="msgType">消息类型</param>
        /// <param name="msg">消息</param>
        /// <returns>YES/NO</returns>
        public static bool UnPack(byte[] bytes, out Type? msgType, out INetMessage? msg)
        {
            msg = null;
            byte[] header = new byte[INT32_LEN];
            byte[] data = new byte[bytes.Length - INT32_LEN];
            Array.Copy(bytes, header, INT32_LEN);
            Array.Copy(bytes, INT32_LEN, data, 0, bytes.Length - INT32_LEN);

            var msgId = BitConverter.ToInt32(header);

            if (messageIdMap.TryGetValue(msgId, out msgType))
            {
                msg = MessagePackSerializer.Deserialize(msgType, data) as INetMessage;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 序列化消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息</param>
        /// <param name="bytes">二进制数据</param>
        /// <returns>YES/NO</returns>
        public static bool Pack<T>(T msg, out byte[]? bytes) where T : INetMessage
        {
            bytes = null;
            var kv = messageIdMap.First(kv => kv.Value.Equals(msg.GetType()));
            if (null != kv.Value)
            {
                var header = BitConverter.GetBytes(kv.Key);
                var data = MessagePackSerializer.Serialize(msg);
                bytes = new byte[header.Length + data.Length];
                Array.Copy(header, 0, bytes, 0, header.Length);
                Array.Copy(data, 0, bytes, header.Length, data.Length);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 协议号定义
        /// </summary>
        private static Dictionary<int, Type> messageIdMap = new()
        {
            {10001, typeof(C2SLoginMsg)},
            {10002, typeof(C2SRegisterMsg)},
            {10003, typeof(S2CLoginMsg)},
            {10004, typeof(S2CRegisterMsg)},
        };
    }
}
