using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Base;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.UI
{
    /// <summary>
    /// Game-UI 游戏 UI 组件
    /// </summary>
    public class GameUI : CComp
    {
        /// <summary>
        /// UILayer 层级
        /// </summary>
        public enum UILayer
        {
            UIMain,
            UIAlert,
            UITop,
        }

        /// <summary>
        /// UIState 状态
        /// </summary>
        public enum UIState
        {
            Free,
            Loading,
            Open,
            Close
        }

        public GameObject UIRoot;
        public Camera UICamrea;

        protected override void OnCreate()
        {
            base.OnCreate();
            UIRoot = GameObject.Find("UI/UIRoot");
            UICamrea = GameObject.Find("UI/UICamera").GetComponent<Camera>();

            // 批量生成 UILayer
            foreach (var name in Enum.GetNames(typeof(UILayer)))
                if (Enum.TryParse(name, out UILayer result))
                    GenUILayerNode(result);
        }

        private Dictionary<UILayer, GameObject> layerNodeDict = new Dictionary<UILayer, GameObject>();

        private void GenUILayerNode(UILayer layer)
        {
            if (layerNodeDict.ContainsKey(layer)) return;

            var layerNode = new GameObject(layer.ToString());
            layerNode.transform.SetParent(UIRoot.transform, true);

            layerNodeDict.Add(layer, layerNode);

            // 设定 RectTransform
            var rtf = layerNode.AddComponent<RectTransform>();
            rtf.sizeDelta = Vector2.zero;
            rtf.anchorMin = Vector2.zero;
            rtf.anchorMax = Vector2.one;
            rtf.localPosition = Vector3.zero;
            rtf.localScale = Vector3.one;

            // 设定 Canvas
            var canvas = layerNode.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = layer.ToString();
        }

        public GameObject GetLayerNode(UILayer layer)
        {
            layerNodeDict.TryGetValue(layer, out GameObject layerNode);

            return layerNode;
        }

        private Dictionary<Type, UIBaseView> viewDict = new Dictionary<Type, UIBaseView>();

        public T GetUI<T>() where T : UIBaseView
        {
            if (viewDict.TryGetValue(typeof(T), out var view)) return view as T;

            return null;
        }

        public async Task<T> LoadUI<T>() where T : UIBaseView, new()
        {
            var view = GetUI<T>();
            if (null != view) return view;

            view = AddComp<T>();
            view.Create();
            await view.Load();

            viewDict.Add(typeof(T), view);

            return view;
        }

        public void UnLoadUI<T>() where T : UIBaseView
        {
            var view = GetUI<T>();
            if (null == view) return;

            view.Unload();
            RmvComp(view);
            view.Destroy();

            viewDict.Remove(typeof(T));
        }

        /// <summary>
        /// UI 当前最顶 Sorting
        /// </summary>
        private int sorting = 0;

        /// <summary>
        /// UI 之间 Sorting 间距
        /// </summary>
        private const int sortingSpacing = 10;

        public async void OpenUI<T>(bool autoload = true) where T : UIBaseView, new()
        {
            var view = GetUI<T>();
            if (null == view)
            {
                if (false == autoload) return;
                view = await LoadUI<T>();
            }

            if (UIState.Open == view.state) return;

            sorting += sortingSpacing;
            view.Sorting = sorting;
            view.Open();
        }

        public async void Close<T>(bool autounload = true) where T : UIBaseView
        {
            var view = GetUI<T>();
            
            if (null == view) return;
            if (UIState.Close == view.state) return;
            
            view.Close();
            UnLoadUI<T>();
        }
    }
}