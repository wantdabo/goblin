using Goblin.Core;
using Godot;

namespace Goblin.Common;

public class GDKit : Comp
{
    public T GetNode<T>(Node node) where T : Node
    {
        if (node is T t) return t;
        return node.GetNodeOrNull<T>(".");
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