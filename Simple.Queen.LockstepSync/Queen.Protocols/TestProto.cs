using MessagePack;
using Queen.Protocols.Common;

namespace Queen.Protocols
{
    [MessagePackObject(true)]
    public class C2STestMsg : INetMessage
    {
        public string text { get; set; }
    }
    
    [MessagePackObject(true)]
    public class S2CTestMsg : INetMessage
    {
        public string text { get; set; }
    }
}
