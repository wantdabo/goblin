using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Godot;

namespace Goblin.Sys.Gameplay.View;

public class ResultView : UIBaseView
{
    public override UILayer layer => UILayer.UIAlert;
    protected override string res => "Gameplay/ResultView";

    private Label resultLabel { get; set; }

    protected override void OnLoad()
    {
        base.OnLoad();
        engine.proxy.gameplay.eventor.Listen<StageResultEvent>(OnStageResult);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        engine.proxy.gameplay.eventor.UnListen<StageResultEvent>(OnStageResult);
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        node.Visible = false;
    }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        resultLabel = engine.gdkit.SeekNode<Label>(node, "ResultLabel");
    }

    protected override void OnBindEvent()
    {
        base.OnBindEvent();
        AddUIEventListener("ReturnBtn", () =>
        {
            engine.gameui.Close(this);
            engine.gameui.Close<HUDView>();
            engine.gameui.Close<GameplayView>();
            engine.gameui.Open<LobbyView>();
            engine.proxy.gameplay.StopGame();
            engine.proxy.gameplay.DestroyGame();
            engine.proxy.gameplay.UnLoad();
        });
    }

    private void OnStageResult(StageResultEvent e)
    {
        if (resultLabel != null)
        {
            resultLabel.Text = e.win ? "胜利" : "失败";
            resultLabel.AddThemeColorOverride("font_color", e.win
                ? new Color(0.824f, 1f, 0f, 1f)
                : new Color(0.851f, 0.208f, 0f, 1f));
        }
        engine.proxy.gameplay.PauseGame();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        node.Visible = true;
    }
}
