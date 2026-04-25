using Goblin.Core;
using Godot;

namespace Goblin.Common;

public class GDKit : Comp
{
    /// <summary>
    /// 获取 Godot 节点/组件（自身或挂载的脚本）
    /// </summary>
    public T GetNode<T>(Node node) where T : Node
    {
        if (node is T t) return t;
        return node.GetNodeOrNull<T>(".");
    }

    /// <summary>
    /// 精准查找 Godot 节点（路径，如 "Parent/Child"）
    /// </summary>
    public T GetNode<T>(Node node, string path) where T : Node
    {
        return node.GetNodeOrNull<T>(path);
    }

    /// <summary>
    /// 模糊查找 Godot 节点（递归按名称匹配）
    /// </summary>
    public T SeekNode<T>(Node node, string nodeName) where T : class
    {
        return node?.FindChild(nodeName, true, false) as T;
    }

    public Vector2 GetLookInput()
    {
        var v = Input.GetVector("look_left", "look_right", "look_up", "look_down");
        if (v == Vector2.Zero)
        {
            var m = Input.GetLastMouseVelocity() * 0.001f;
            v = new Vector2(m.X, m.Y);
        }
        return v;
    }

    public float GetScrollInput()
    {
        if (Input.IsActionJustReleased("scroll_up")) return 1f;
        if (Input.IsActionJustReleased("scroll_down")) return -1f;
        return 0f;
    }
}