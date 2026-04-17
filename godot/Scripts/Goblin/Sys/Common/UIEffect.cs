using Goblin.Common.GameRes;
using Goblin.Core;
using Godot;

namespace Goblin.Sys.Common
{
    public class UIEffectController
    {
        public GpuParticles3D[] particles;
        public AnimationPlayer[] animplayers;
        public Control node;
        public float duration;
        public string layerName;
        public int sorting;

        public void Stop()
        {
            if (null != node) node.Visible = false;
        }

        public void Play(string stateName = "")
        {
            if (null != node) node.Visible = true;
            if (animplayers != null)
                foreach (var ap in animplayers)
                    if (ap != null && ap.HasAnimation(stateName)) ap.Play(stateName);
        }

        public void AdjSorting() { }
    }

    public class UIEffect : Comp
    {
        private UIEffectController uiec;
        public UIEffectController UIEC => uiec;

        public void Load(Control parentNode, string res)
        {
            var scene = engine.gameres.LoadAssetSync<PackedScene>(Location.uieffectpath + res + ".tscn");
            var effNode = scene?.Instantiate<Control>();
            uiec = new UIEffectController { node = effNode };
            if (null != effNode) parentNode?.AddChild(effNode);
            Stop();
        }

        public void Sorting(string layerName, int sorting)
        {
            uiec.layerName = layerName;
            uiec.sorting = sorting;
            uiec.AdjSorting();
        }

        public void Stop()
        {
            engine.ticker.StopTimer(delayTimingId);
            uiec?.Stop();
        }

        private uint delayTimingId;

        public void Play(string stateName = "")
        {
            if (uiec?.duration > 0)
                engine.ticker.Timing((t) => Stop(), uiec.duration, 1);
            delayTimingId = engine.ticker.Timing((t) => uiec?.Play(stateName), 0.05f, 1);
        }
    }
}
