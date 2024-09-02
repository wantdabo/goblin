using MessagePack;
using MessagePack.Resolvers;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Protocols.Common
{
    /// <summary>
    /// 消息结构接口
    /// </summary>
    public interface INetMessage
    {
    }

    /// <summary>
    /// 协议序列化
    /// </summary>
    public partial class ProtoPack
    {
        /// <summary>
        /// UInt16 字节数量
        /// </summary>
        public static byte UINT16_LEN = 2;

        /// <summary>
        /// Int32 字节数量
        /// </summary>
        public static byte INT32_LEN = 4;

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
            msgType = null;
            try
            {
                byte[] proto = new byte[UINT16_LEN];
                byte[] data = new byte[bytes.Length - UINT16_LEN];
                Array.Copy(bytes, proto, UINT16_LEN);
                Array.Copy(bytes, UINT16_LEN, data, 0, bytes.Length - UINT16_LEN);
                var msgId = BitConverter.ToUInt16(proto);
                if (false == messageDict.TryGetValue(msgId, out msgType)) return false;

                msgType = messageDict[msgId];
                msg = MessagePackSerializer.Deserialize(msgType, data) as INetMessage;
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
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
            var kv = messageDict.FirstOrDefault((kv) => kv.Value == msg.GetType());
            if (null == kv.Value) return false;
            var proto = BitConverter.GetBytes(kv.Key);
            var data = MessagePackSerializer.Serialize(msg);
            bytes = new byte[proto.Length + data.Length];
            Array.Copy(proto, 0, bytes, 0, proto.Length);
            Array.Copy(data, 0, bytes, proto.Length, data.Length);

            return true;
        }

#if GOBLIN
        /// <summary>
        /// 初始化自定义解析器
        /// </summary>
        static ProtoPack()
        {
            StaticCompositeResolver.Instance.Register(
                GeneratedResolver.Instance,
                StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
            MessagePackSerializer.DefaultOptions = option;
        }
#endif
    }
}
