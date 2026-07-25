using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.Commands;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Godot;
using System.Collections.Generic;

namespace Goblin.Sys.Gameplay.View;

public class GameplayView : UIBaseView
{
    public override UILayer layer => UILayer.UIMain;
    protected override string res => "Gameplay/GameplayView";

    private Label synopsisText { get; set; }
    private HSlider gameSpeedSlider { get; set; }
    private Label gameSpeedDescText { get; set; }
    private CheckBox gamingCBToggle { get; set; }
    private CheckBox physDrawerToggle { get; set; }
    private CheckBox showInfoCbToggle { get; set; }
    private CheckBox danceCBToggle { get; set; }
    private CheckBox enemyAutopoilotToggle { get; set; }
    private Control selfSeatPoint { get; set; }
    private Control infoContent { get; set; }
    private Control infoOrg { get; set; }
    private List<Control> infoItems = new();
    private bool mlmbprev { get; set; }

    protected override void OnLoad()
    {
        base.OnLoad();
        engine.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        engine.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        synopsisText = engine.gdkit.SeekNode<Label>(node, "Synopsis");
        gameSpeedSlider = engine.gdkit.SeekNode<HSlider>(node, "GameSpeedSlider");
        gameSpeedDescText = engine.gdkit.SeekNode<Label>(node, "GameSpeedDesc");
        gamingCBToggle = engine.gdkit.SeekNode<CheckBox>(node, "GamingCB");
        physDrawerToggle = engine.gdkit.SeekNode<CheckBox>(node, "PhysDrawerCB");
        showInfoCbToggle = engine.gdkit.SeekNode<CheckBox>(node, "ShowInfoCB");
        danceCBToggle = engine.gdkit.SeekNode<CheckBox>(node, "DanceCB");
        enemyAutopoilotToggle = engine.gdkit.SeekNode<CheckBox>(node, "EnemyAutopoilotCB");
        selfSeatPoint = engine.gdkit.SeekNode<Control>(node, "SelfSeatPoint");
        infoContent = engine.gdkit.SeekNode<Control>(node, "InfoContent");
        infoOrg = engine.gdkit.SeekNode<Control>(node, "InfoOrgGo");
    }

    protected override void OnBindEvent()
    {
        base.OnBindEvent();
        AddUIEventListener("EnterLockCursorBtn", () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        });
        if (gameSpeedSlider != null)
            gameSpeedSlider.ValueChanged += (v) =>
            {
                var local = engine.proxy.gameplay.director as LocalDirector;
                if (null == local) return;
                var timescale = Mathf.Round((float)v / 0.25f) * 0.25f;
                var cmd = ObjectPool.Ensure<TimeScaleCommand>();
                cmd.timescale = (int)(timescale * 1000);
                local.world.input.EnqueueCommand(cmd);
                if (gameSpeedDescText != null) gameSpeedDescText.Text = timescale.ToString();
            };
        AddUIEventListener("GamingCB", () =>
        {
            if (gamingCBToggle?.ButtonPressed == true) engine.proxy.gameplay.director.ResumeGame();
            else engine.proxy.gameplay.director.PauseGame();
        });
        AddUIEventListener("PhysDrawerCB", () => { engine.proxy.gameplay.physdraw = physDrawerToggle?.ButtonPressed ?? false; });
        AddUIEventListener("ShowInfoCB", () => { engine.proxy.gameplay.showinfo = showInfoCbToggle?.ButtonPressed ?? false; });
        AddUIEventListener("DanceCB", () => { engine.proxy.gameplay.dancing = danceCBToggle?.ButtonPressed ?? false; });
        AddUIEventListener("EnemyAutopoilotCB", () => { engine.proxy.gameplay.enemyautopilot = enemyAutopoilotToggle?.ButtonPressed ?? false; });
        AddUIEventListener("SwitchSeatBtn", () =>
        {
            var world = engine.proxy.gameplay.director.world;
            var seat = world.selfseat == 1 ? 2ul : 1ul;
            world.SwitchSeat(seat);
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = $"切换成功, 座位 {seat}" });
        });
        AddUIEventListener("SnapshotBtn", () =>
        {
            engine.proxy.gameplay.director.Snapshot();
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照拍摄成功." });
        });
        AddUIEventListener("RestoreBtn", () =>
        {
            engine.proxy.gameplay.director.Restore();
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照恢复成功." });
        });
        AddUIEventListener("ExitBtn", () =>
        {
            engine.gameui.Close(this);
            engine.gameui.Close<HUDView>();
            engine.gameui.Close<ResultView>();
            engine.gameui.Open<LobbyView>();
            engine.proxy.gameplay.director.StopGame();
            engine.proxy.gameplay.director.DestroyGame();
            engine.proxy.gameplay.UnLoad();
        });
    }

    private void OnLateTick(LateTickEvent e)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            Input.MouseMode = Input.MouseModeEnum.Visible;

        if (Input.MouseMode == Input.MouseModeEnum.Visible)
        {
            var lmb = Input.IsMouseButtonPressed(MouseButton.Left);
            if (lmb && false == mlmbprev)
            {
                var hovered = node?.GetViewport()?.GuiGetHoveredControl();
                if (null == hovered) Input.MouseMode = Input.MouseModeEnum.Captured;
            }
            mlmbprev = lmb;
        }
        else mlmbprev = false;

        var director = engine.proxy.gameplay.director;
        if (null == director) return;

        // Phase 2+：在此从 renderworld.Entity.Component 读取数据绘制 HUD
        // Phase 1：仅展示 synopsis 基本帧信息

        if (null != synopsisText)
            synopsisText.Text =
                $"单步耗时 (毫秒) : {director.stepms}\n" +
                $"时间缩放 : {director.timescale}";
    }
}
