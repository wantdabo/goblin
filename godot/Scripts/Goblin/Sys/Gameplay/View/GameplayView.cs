using Goblin.Common;
using Goblin.Gameplay.Logic.Commands;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Projection.Shadows;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Godot;
using Kowtow.Math;
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
                var proxy = engine.proxy.gameplay;
                var timescale = Mathf.Round((float)v / 0.25f) * 0.25f;
                var cmd = ObjectPool.Ensure<TimeScaleCommand>();
                cmd.timescale = (int)(timescale * 1000);
                proxy.input.EnqueueCommand(cmd);
                if (gameSpeedDescText != null) gameSpeedDescText.Text = timescale.ToString();
            };
        AddUIEventListener("GamingCB", () =>
        {
            if (gamingCBToggle?.ButtonPressed == true) engine.proxy.gameplay.ResumeGame();
            else engine.proxy.gameplay.PauseGame();
        });
        AddUIEventListener("PhysDrawerCB", () => { engine.proxy.gameplay.physdraw = physDrawerToggle?.ButtonPressed ?? false; });
        AddUIEventListener("ShowInfoCB", () => { engine.proxy.gameplay.showinfo = showInfoCbToggle?.ButtonPressed ?? false; });
        AddUIEventListener("DanceCB", () => { engine.proxy.gameplay.dancing = danceCBToggle?.ButtonPressed ?? false; });
        AddUIEventListener("EnemyAutopoilotCB", () => { engine.proxy.gameplay.enemyautopilot = enemyAutopoilotToggle?.ButtonPressed ?? false; });
        AddUIEventListener("SwitchSeatBtn", () =>
        {
            var proxy = engine.proxy.gameplay;
            var seat = proxy.selfseat == 1 ? 2ul : 1ul;
            proxy.SwitchSeat(seat);
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = $"切换成功, 座位 {seat}" });
        });
        AddUIEventListener("SnapshotBtn", () =>
        {
            engine.proxy.gameplay.Snapshot();
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照拍摄成功." });
        });
        AddUIEventListener("RestoreBtn", () =>
        {
            engine.proxy.gameplay.Restore();
            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照恢复成功." });
        });
        AddUIEventListener("ExitBtn", () =>
        {
            engine.gameui.Close(this);
            engine.gameui.Close<HUDView>();
            engine.gameui.Close<ResultView>();
            engine.gameui.Open<LobbyView>();
            engine.proxy.gameplay.StopGame();
            engine.proxy.gameplay.DestroyGame();
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

        var proxy = engine.proxy.gameplay;
        if (null == proxy.stage) return;

        // 捕获键盘输入，写入 InputSystem
        CaptureInput(proxy);

        // 主线程应用投影管线产出的观察者包
        proxy.ApplyProjection();

        if (null != synopsisText)
        {
            var hudText = "";
            var hero = proxy.stage.seat.GetActor(proxy.selfseat);
            var hud = proxy.canvas?.GetShadow<HUDShadow>(hero);
            if (null != hud)
            {
                hudText = $"HP: {hud.hp}/{hud.maxhp}  Atk: {hud.attack}  Spd: {hud.movespeed}\n";
            }

            var spatialText = "";
            var spatial = proxy.canvas?.GetShadow<SpatialShadow>(hero);
            if (null != spatial)
            {
                spatialText = $"POS: ({spatial.position.x:F1},{spatial.position.y:F1},{spatial.position.z:F1})  " +
                    $"EUL: ({spatial.euler.x:F1},{spatial.euler.y:F1},{spatial.euler.z:F1})\n";
            }

            // Facade 信息
            var facadeText = "";
            var facade = proxy.canvas?.GetShadow<FacadeShadow>(hero);
            if (null != facade)
            {
                facadeText = $"Model: {facade.model}  Anim: {facade.animstate}/{facade.animhash}@{facade.animelapsed}  " +
                    $"AnimsSlot: {facade.animslots?.Count ?? 0}  " +
                    $"Effect: +{facade.effectincrement} ({(facade.effectdict?.Count ?? 0)})\n";
            }

            synopsisText.Text =
                $"单步耗时 (毫秒) : {proxy.stepms}\n" +
                $"时间缩放 : {proxy.timescale}\n" +
                hudText +
                spatialText +
                facadeText;
        }
    }

    /// <summary>
    /// 捕获键盘输入，写入 InputSystem 输入槽
    /// </summary>
    private void CaptureInput(GameplayProxy proxy)
    {
        // 摇杆：WASD → IntVector2 方向
        var x = 0;
        var y = 0;
        if (Input.IsKeyPressed(Key.D)) x += 1;
        if (Input.IsKeyPressed(Key.A)) x -= 1;
        if (Input.IsKeyPressed(Key.S)) y += 1;
        if (Input.IsKeyPressed(Key.W)) y -= 1;
        var hasDirection = 0 != x || 0 != y;
        proxy.input.SetInput(INPUT_DEFINE.JOYSTICK, hasDirection, new IntVector2(x * 1000, y * 1000));

        // 攻击：J 键 或 鼠标左键
        var baPressed = Input.IsKeyPressed(Key.J) || Input.IsMouseButtonPressed(MouseButton.Left);
        proxy.input.SetInput(INPUT_DEFINE.BA, baPressed, new IntVector2());

        // 技能 1：K 键
        var bbPressed = Input.IsKeyPressed(Key.K);
        proxy.input.SetInput(INPUT_DEFINE.BB, bbPressed, new IntVector2());

        // 技能 2：L 键
        var bcPressed = Input.IsKeyPressed(Key.L);
        proxy.input.SetInput(INPUT_DEFINE.BC, bcPressed, new IntVector2());
    }
}
