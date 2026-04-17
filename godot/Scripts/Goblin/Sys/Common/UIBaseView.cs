using Goblin.Common.GameRes;
using Godot;
using System.Threading.Tasks;

namespace Goblin.Sys.Common
{
    public enum UILayer { UIMain, UIAlert, UITop }

    public enum UIState { Free, Loading, Loaded, Open, Close }

    public abstract class UIBaseView : UIBase<UIBaseView>
    {
        public abstract UILayer layer { get; }
        public UIState state { get; private set; }
        public virtual bool quickclose { get; } = true;

        private string mLayerName;
        public override string layerName
        {
            get => mLayerName;
            set
            {
                mLayerName = value;
                if (node?.GetParent() is CanvasLayer cl) cl.Name = mLayerName;
            }
        }

        private int mSorting;
        public override int sorting
        {
            get => mSorting;
            set
            {
                mSorting = value;
                if (node?.GetParent() is CanvasLayer cl) cl.Layer = mSorting;
            }
        }

        protected object[] args;

        public async Task<UIBaseView> Load()
        {
            state = UIState.Loading;
            var layerNode = engine.gameui.GetLayerNode(layer);
            var scene = engine.gameres.LoadAssetSync<PackedScene>(Location.uiprefabpath + res + ".tscn");
            node = scene?.Instantiate<Control>();
            if (null != node) layerNode?.AddChild(node);

            OnLoad();
            OnBuildUI();
            OnBindEvent();
            state = UIState.Loaded;
            return this;
        }

        public void Open(params object[] args)
        {
            this.args = args;
            if (UIState.Loading == state) return;
            if (UIState.Open == state) Close();
            layerName = layer.ToString();
            sorting = engine.gameui.AllotSorting();
            OnOpen();
        }

        protected override void OnUnload() { state = UIState.Free; base.OnUnload(); }

        protected override void OnOpen()
        {
            state = UIState.Open;
            node.Visible = true;
            base.OnOpen();
        }

        protected override void OnClose()
        {
            state = UIState.Close;
            node.Visible = false;
            engine.gameui.Close(GetType());
        }
    }
}
