using Goblin.Common;

namespace Goblin.Sys.Common;

public struct MessageBlowEvent : IEvent
{
    public int type;
    public string desc;
}