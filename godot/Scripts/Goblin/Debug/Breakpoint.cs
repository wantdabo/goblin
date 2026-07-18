using System.Text.Json.Nodes;

namespace Goblin.Debug;

/// <summary>
/// 断点条件——LLM 可通过 HTTP 设置，触发时游戏自动暂停。
/// </summary>
public class Breakpoint
{
    public enum BreakpointType
    {
        /// <summary>状态变更：当任意/指定 Actor 进入目标状态时暂停</summary>
        StateChange,
        /// <summary>属性阈值：当任意/指定 Actor 的属性满足比较条件时暂停</summary>
        Attribute,
        /// <summary>帧计数：达到指定帧号时暂停</summary>
        Frame,
    }

    public BreakpointType type { get; set; }
    /// <summary>目标 Actor ID，"*" 表示所有</summary>
    public string actorfilter { get; set; } = "*";
    /// <summary>目标状态名（StateChange 用）</summary>
    public string targetstate { get; set; } = "";
    /// <summary>属性名（Attribute 用）</summary>
    public string attrname { get; set; } = "";
    /// <summary>比较操作符："lt"/"le"/"gt"/"ge"/"eq"/"ne"</summary>
    public string op { get; set; } = "";
    /// <summary>比较值（Attribute 用）</summary>
    public int value { get; set; }
    /// <summary>目标帧号（Frame 用）</summary>
    public uint targetframe { get; set; }

    /// <summary>
    /// 根据当前快照评估断点是否命中。
    /// </summary>
    public bool Evaluate(IStateProvider state)
    {
        switch (type)
        {
            case BreakpointType.Frame:
                return EvaluateFrame(state);
            case BreakpointType.StateChange:
                return EvaluateStateChange(state);
            case BreakpointType.Attribute:
                return EvaluateAttribute(state);
            default:
                return false;
        }
    }

    private bool EvaluateFrame(IStateProvider state)
    {
        JsonObject snapshot = state.Snapshot();
        return snapshot["frame"]?.GetValue<uint>() >= targetframe;
    }

    private bool EvaluateStateChange(IStateProvider state)
    {
        JsonArray machines = state.GetStateMachines();
        foreach (JsonNode item in machines)
        {
            JsonObject itemobj = item!.AsObject();
            if (false == MatchActor(itemobj)) continue;

            if (itemobj["current"]?.GetValue<string>() == targetstate)
                return true;
        }
        return false;
    }

    private bool EvaluateAttribute(IStateProvider state)
    {
        JsonObject snapshot = state.Snapshot();
        JsonArray actors = snapshot["actors"]?.AsArray();
        if (null == actors) return false;

        foreach (JsonNode actor in actors)
        {
            ulong id = actor!["id"]?.GetValue<ulong>() ?? 0;
            if (false == MatchActorId(id)) continue;

            JsonObject attrs = state.GetAttributes(id);
            if (false == attrs.TryGetPropertyValue(attrname, out JsonNode attrnode) || null == attrnode) continue;

            int attrvalue;
            if (attrnode is JsonObject obj)
                attrvalue = obj["value"]?.GetValue<int>() ?? obj["raw"]?.GetValue<int>() ?? 0;
            else
                attrvalue = attrnode.GetValue<int>();

            if (Compare(attrvalue, value, op))
                return true;
        }
        return false;
    }

    private bool MatchActor(JsonObject actorobj)
    {
        if ("*" == actorfilter) return true;
        if (ulong.TryParse(actorfilter, out ulong targetid))
            return actorobj["id"]?.GetValue<ulong>() == targetid;
        if (actorobj.TryGetPropertyValue("type", out JsonNode typenode))
            return typenode?.GetValue<string>() == actorfilter;
        return false;
    }

    private bool MatchActorId(ulong id)
    {
        if ("*" == actorfilter) return true;
        return ulong.TryParse(actorfilter, out ulong targetid) && id == targetid;
    }

    private static bool Compare(int a, int b, string op)
    {
        return op switch
        {
            "lt" => a < b,
            "le" => a <= b,
            "gt" => a > b,
            "ge" => a >= b,
            "eq" => a == b,
            "ne" => a != b,
            _ => false,
        };
    }
}
