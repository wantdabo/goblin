using Goblin.Common.GameRes;
using Godot;
using System.Threading.Tasks;

namespace Goblin.Sys.Common;

public abstract class UIBaseCell : UIBase<UIBaseCell>
{
    public Control container { get; set; }
    private bool active { get; set; } = true;
    protected bool isActive => active;

    public void SetActive(bool status)
    {
        active = status;
        if (null != node) node.Visible = active;
        OnActive();
    }

    public void SetParent(Control parent)
    {
        if (null == parent) return;
        node?.GetParent()?.RemoveChild(node);
        parent.AddChild(node);
        container = parent;
    }

    public async Task<UIBaseCell> Load()
    {
        var scene = engine.gameres.LoadAssetSync<PackedScene>(Location.uiprefabpath + res + ".tscn");
        node = scene?.Instantiate<Control>();
        if (null != node) container?.AddChild(node);
        OnLoad();
        OnBuildUI();
        OnBindEvent();
        return this;
    }

    public void Open() => OnOpen();

    protected virtual void OnActive() { }
}