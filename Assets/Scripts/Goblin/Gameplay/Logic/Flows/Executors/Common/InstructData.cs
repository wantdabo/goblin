using Goblin.Gameplay.Logic.Flows.Defines;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Common
{
    /// <summary>
    /// 指令数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class InstructData
    {
        /// <summary>
        /// 指令 ID
        /// </summary>
        public abstract ushort id { get; }

        /// <summary>
        /// 执行目标
        /// </summary>
        [LabelText("执行目标")]
        [ValueDropdown("@OdinValueDropdown.GetExecuteTargetDefine()")]
        public byte et = FLOW_DEFINE.ET_FLOW_OWNER;

        /// <summary>
        /// 序列化指令数据
        /// </summary>
        /// <returns>二进制数据</returns>
        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this as object);
        }
    }
}