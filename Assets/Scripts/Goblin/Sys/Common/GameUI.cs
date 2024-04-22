using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Common;
using UnityEngine;
using Goblin.Sys.Common;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// UILayer 层级
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 主要
        /// </summary>
        UIMain,
        /// <summary>
        /// 弹窗
        /// </summary>
        UIAlert,
        /// <summary>
        /// 顶层
        /// </summary>
        UITop,
    }

    /// <summary>
    /// UIState 状态
    /// </summary>
    public enum UIState
    {
        /// <summary>
        /// 空闲
        /// </summary>  
        Free,
        /// <summary>
        /// 加载
        /// </summary>
        Loading,
        /// <summary>
        /// 加载完毕
        /// </summary>
        Loaded,
        /// <summary>
        /// 打开
        /// </summary>
        Open,
        /// <summary>
        /// 关闭
        /// </summary>
        Close
    }

    /// <summary>
    /// UI 管理
    /// </summary>
    public class GameUI : Comp
    {
        public GameObject uiroot;
        public Camera uicamera;

        protected override void OnCreate()
        {
            base.OnCreate();
            uiroot = GameObject.Find("UI/UIRoot");
            uicamera = GameObject.Find("UI/UICamera").GetComponent<Camera>();

            // 批量生成 UILayer
            foreach (var name in Enum.GetNames(typeof(UILayer)))
            {
                if (Enum.TryParse(name, out UILayer result))
                {
                    GenUILayerNode(result);
                }
            }
        }

        private Dictionary<UILayer, GameObject> layerNodeDict = new();

        private void GenUILayerNode(UILayer layer)
        {
            if (layerNodeDict.ContainsKey(layer)) return;

            var layerNode = new GameObject(layer.ToString());
            layerNode.transform.SetParent(uiroot.transform, true);

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

        private Dictionary<Type, UIBaseView> viewDict = new();

        public T Get<T>() where T : UIBaseView
        {
            if (viewDict.TryGetValue(typeof(T), out var view)) return view as T;

            return null;
        }

        public async Task<T> Load<T>() where T : UIBaseView, new()
        {
            var view = Get<T>();
            if (null != view) return view;

            view = AddComp<T>();
            view.Create();
            viewDict.Add(typeof(T), view);
            await view.Load();

            return view;
        }

        public void UnLoad<T>() where T : UIBaseView
        {
            var view = Get<T>();
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

        public async void Open<T>(bool autoload = true) where T : UIBaseView, new()
        {
            var view = Get<T>();
            if (null == view)
            {
                if (false == autoload) return;
                view = await Load<T>();
            }

            if (UIState.Loading == view.state) return;
            if (UIState.Open == view.state) return;

            sorting += sortingSpacing;
            view.layerName = view.layer.ToString();
            view.sorting = sorting;
            view.Open();
        }

        /// <summary>
        /// 关闭 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="autounload">是否自动卸载</param>
        public void Close<T>(bool autounload = true) where T : UIBaseView
        {
            var view = Get<T>();

            if (null == view) return;
            if (UIState.Close == view.state) return;

            view.Close();
            if(autounload) UnLoad<T>();
        }
    }
}