using System.Text.Json.Nodes;

namespace Goblin.Debug;

/// <summary>
/// 状态提供者接口——DebugServer 通过此接口读取运行时状态，与具体层的实现解耦。
/// 当前实现：GameplayStateProvider（基于 Stage 公共 API）。
/// 未来可扩展：SystemStateProvider（系统层）。
/// </summary>
public interface IStateProvider
{
    /// <summary>
    /// 构建一次完整快照（JSON），包含所有 Actor 的状态摘要。
    /// </summary>
    JsonObject Snapshot();
    /// <summary>
    /// 构建指定 Actor 的完整状态（JSON）。
    /// </summary>
    JsonObject GetActor(ulong actorid);
    /// <summary>
    /// 构建所有 Actor 摘要列表。
    /// </summary>
    JsonArray GetActorSummaries();
    /// <summary>
    /// 构建所有存活 Actor 的状态机摘要。
    /// </summary>
    JsonArray GetStateMachines();
    /// <summary>
    /// 构建指定 Actor 的管线状态。
    /// </summary>
    JsonObject GetFlow(ulong actorid);
    /// <summary>
    /// 构建指定 Actor 的属性。
    /// </summary>
    JsonObject GetAttributes(ulong actorid);
}
