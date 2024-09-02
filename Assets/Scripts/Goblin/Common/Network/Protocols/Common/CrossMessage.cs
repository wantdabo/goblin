using MessagePack;

namespace Queen.Protocols.Common
{
    [MessagePackObject(true)]
    public class ReqCrossMessage : INetMessage
    {
        public string route { get; set; }
        public string content { get; set; }
    }

    [MessagePackObject(true)]
    public class ResCrossMessage : INetMessage
    {
        public ushort state { get; set; }
        public string content { get; set; }
    }
}
