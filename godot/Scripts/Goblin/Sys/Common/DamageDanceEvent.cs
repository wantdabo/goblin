using Godot;

namespace Goblin.Sys.Common;

public struct CureDanceEvent : Goblin.Common.IEvent
{
    public Vector2 screenpos;
    public uint cure;
    public ulong from;
    public ulong to;
}

public struct DamageDanceEvent : Goblin.Common.IEvent
{
    public Vector2 screenpos;
    public bool crit;
    public int damage;
    public ulong from;
    public ulong to;
}