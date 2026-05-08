using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.Commands;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common.Extensions;
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

        var local = engine.proxy.gameplay.director as LocalDirector;
        if (null == local) return;

        var spatial = local.world.rilbucket.GetRIL<RIL_SPATIAL>(local.world.self);
        if (null != spatial && selfSeatPoint != null)
        {
            var cam = local.world.eyes.camera;
            var worldPos = spatial.position.ToVector3() + new Vector3(0, 2f, 0);
            var screenPos = cam.UnprojectPosition(worldPos);
            var localPos = selfSeatPoint.GetParent<Control>().GetGlobalTransformWithCanvas().AffineInverse() * screenPos;
            selfSeatPoint.Position = selfSeatPoint.Position.Lerp(localPos, 0.1f);
        }

        foreach (var item in infoItems) if (item.Visible) item.Visible = false;
        if (engine.proxy.gameplay.showinfo && infoContent != null && infoOrg != null)
        {
            var rilactors = local.world.rilbucket.GetRIL<RIL_ACTOR>(local.world.sa);
            var cam = local.world.eyes.camera;
            int index = 0;
            if (null == rilactors) goto synopsis;
            foreach (var actor in rilactors.actors)
            {
                var rilspatial = local.world.rilbucket.GetRIL<RIL_SPATIAL>(actor);
                if (null == rilspatial) continue;
                var worldPos = rilspatial.position.ToVector3();
                if (cam.IsPositionBehind(worldPos)) continue;
                var sp = cam.UnprojectPosition(worldPos);
                var vp = sp / cam.GetViewport().GetVisibleRect().Size;
                if (vp.X < 0 || vp.X > 1 || vp.Y < 0 || vp.Y > 1) continue;

                var rilsm = local.world.rilbucket.GetRIL<RIL_STATE_MACHINE>(actor);
                var rilticker = local.world.rilbucket.GetRIL<RIL_TICKER>(actor);
                var rilattr = local.world.rilbucket.GetRIL<RIL_ATTRIBUTE>(actor);
                var color = local.world.self == actor ? "#D2FF00" : "#B90000";
                string info = $"[color={color}]ACTOR : {actor}\n";
                if (null != rilsm) { info += $"当前状态 : {rilsm.current}\n"; info += $"之前状态 : {rilsm.last}\n"; }
                if (null != rilticker) info += $"TIMESCALE : {rilticker.timescale}\n";
                if (null != rilattr) { info += $"当前生命值 : {rilattr.hp}\n最大生命值 : {rilattr.maxhp}\n移动速度 : {rilattr.movespeed}\n攻击力 : {rilattr.attack}\n"; }
                info += "[/color]";

                if (index >= infoItems.Count)
                {
                    var newItem = (Control)infoOrg.Duplicate();
                    infoContent.AddChild(newItem);
                    infoItems.Add(newItem);
                }
                var infoItem = infoItems[index];
                infoItem.Visible = true;
                if (infoItem.GetChildCount() > 0 && infoItem.GetChild(0) is RichTextLabel rtl) rtl.Text = info;
                else if (infoItem is RichTextLabel selfRtl) selfRtl.Text = info;

                var infoWorldPos = rilspatial.position.ToVector3() + new Vector3(0, 1f, 0);
                float dist = cam.GlobalPosition.DistanceTo(infoWorldPos);
                infoItem.Scale = Vector2.One * (10f / (dist + 1f));
                var uiPos = infoContent.GetGlobalTransformWithCanvas().AffineInverse() * cam.UnprojectPosition(infoWorldPos);
                infoItem.Position = infoItem.Position.Lerp(uiPos, 0.2f);
                index++;
            }
        }

        synopsis:
        var rilstage = local.world.rilbucket.GetRIL<RIL_STAGE>(local.world.sa);
        if (null == rilstage || synopsisText == null) return;
        synopsisText.Text =
            $"帧号 : {rilstage.frame}\n" +
            $"逻辑耗时 (毫秒) : {local.stepms}\n" +
            $"Actor : {rilstage.actorcnt}\n" +
            $"Behavior : {rilstage.behaviorcnt}\n" +
            $"BehaviorInfo : {rilstage.behaviorinfocnt}\n" +
            "存在快照 : " + (rilstage.hassnapshot ? "是\n" : "否\n") +
            (rilstage.hassnapshot ? $"快照帧号 : {rilstage.snapshotframe}" : "");
        if (!rilstage.hassnapshot || rilstage.frame - rilstage.snapshotframe > 1) return;
        if (gameSpeedSlider != null) gameSpeedSlider.Value = local.timescale;
        if (gameSpeedDescText != null) gameSpeedDescText.Text = local.timescale.ToString();
    }
}