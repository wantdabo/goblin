using Goblin.Core;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goblin.Sys.Common
{
    public class GameUI : Comp
    {
        private Dictionary<UILayer, CanvasLayer> layerDict = new();
        private Dictionary<Type, UIBaseView> viewDict = new();
        public int sorting { get; private set; } = 0;
        public int sortingSpacing { get; private set; } = 10;

        protected override void OnCreate()
        {
            base.OnCreate();
            var root = Godot.Engine.GetMainLoop() is SceneTree st ? st.Root : null;
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var cl = new CanvasLayer { Name = layer.ToString(), Layer = (int)layer };
                root?.AddChild(cl);
                layerDict[layer] = cl;
            }
        }

        public CanvasLayer GetLayerNode(UILayer layer)
        {
            layerDict.TryGetValue(layer, out var cl);
            return cl;
        }

        public int AllotSorting() { sorting += sortingSpacing; return sorting; }

        public T Get<T>() where T : UIBaseView => viewDict.TryGetValue(typeof(T), out var v) ? v as T : null;
        public UIBaseView Get(Type type) => viewDict.TryGetValue(type, out var v) ? v : null;

        public async Task<T> Load<T>() where T : UIBaseView, new()
        {
            var view = Get<T>();
            if (null != view) return view;
            view = AddComp<T>();
            view.Create();
            viewDict[typeof(T)] = view;
            await view.Load();
            return view;
        }

        public void Unload<T>() where T : UIBaseView => Unload(typeof(T));
        public void Unload(Type type)
        {
            var view = Get(type);
            if (null == view) return;
            view.Unload(); view.Destroy();
            viewDict.Remove(type);
        }

        public async void Open<T>(params object[] param) where T : UIBaseView, new()
        {
            var view = Get<T>() ?? await Load<T>();
            view.Open(param);
        }

        public void Close<T>(bool autounload = true) where T : UIBaseView => Close(typeof(T), autounload);
        public void Close(Type type, bool autounload = true)
        {
            var view = Get(type);
            if (null == view || UIState.Close == view.state) return;
            view.Close();
            if (autounload) Unload(type);
        }
        public void Close<T>(T view, bool autounload = true) where T : UIBaseView => Close(view.GetType(), autounload);

        public void QuickClose(bool autounload = true)
        {
            foreach (var type in viewDict.Keys.ToArray())
            {
                var view = Get(type);
                if (null != view && view.quickclose) Close(type, autounload);
            }
        }
    }
}
