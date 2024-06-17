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
        /// <summary>
        /// UI 根节点
        /// </summary>
        public GameObject uiroot { get; private set; }
        /// <summary>
        /// UI 相机
        /// </summary>
        public Camera uicamera { get; private set; }

        /// <summary>
        /// UI 层级根节点
        /// </summary>
        private Dictionary<UILayer, GameObject> layerNodeDict = new();

        /// <summary>
        /// UI 界面集合
        /// </summary>
        private Dictionary<Type, UIBaseView> viewDict = new();

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

        /// <summary>
        /// 生成 UI 层级根节点
        /// </summary>
        /// <param name="layer">UI 层级</param>
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

        /// <summary>
        /// 获取 UI 层级根节点
        /// </summary>
        /// <param name="layer">UI 层级</param>
        /// <returns>UI 层级根节点</returns>
        public GameObject GetLayerNode(UILayer layer)
        {
            layerNodeDict.TryGetValue(layer, out GameObject layerNode);

            return layerNode;
        }

        /// <summary>
        /// 获取 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns>UI 界面</returns>
        public T Get<T>() where T : UIBaseView
        {
            var view = Get(typeof(T));
            if (null == view) return null;

            return view as T;
        }

        /// <summary>
        /// 获取 UI 界面
        /// </summary>
        /// <param name="type">界面类型</param>
        /// <returns>UI 界面</returns>
        public UIBaseView Get(Type type)
        {
            if (viewDict.TryGetValue(type, out var view)) return view;

            return null;
        }

        /// <summary>
        /// 加载 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns>UI 界面</returns>
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

        /// <summary>
        /// 卸载 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</param>
        public void Unload<T>() where T : UIBaseView
        {
            Unload(typeof(T));
        }

        /// <summary>
        /// 卸载 UI 界面
        /// </summary>
        /// <param name="type">界面类型</param>
        public void Unload(Type type)
        {
            var view = Get(type);
            if (null == view) return;

            view.Unload();
            view.Destroy();

            viewDict.Remove(type);
        }

        /// <summary>
        /// UI 当前最顶 Sorting
        /// </summary>
        public int sorting { get; private set; } = 0;

        /// <summary>
        /// UI 之间 Sorting 间距
        /// </summary>
        public int sortingSpacing { get; private set; } = 10;

        /// <summary>
        /// UI 根据 Sorting 间距分配 Sorting
        /// </summary>
        /// <returns>分配 Sorting</returns>
        public int AllotSorting()
        {
            sorting += sortingSpacing;

            return sorting;
        }

        /// <summary>
        /// 打开 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="autoload">自动加载</param>
        public async void Open<T>(bool autoload = true) where T : UIBaseView, new()
        {
            var view = Get<T>();
            if (null == view)
            {
                if (false == autoload) return;
                view = await Load<T>();
            }
            view.Open();
        }

        /// <summary>
        /// 关闭 UI 界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="autounload">是否自动卸载</param>
        public void Close<T>(bool autounload = true) where T : UIBaseView
        {
            Close(typeof(T), autounload);
        }

        /// <summary>
        /// 关闭 UI 界面
        /// </summary>
        /// <param name="type">界面类型</param>
        /// <param name="autounload">是否自动卸载</param>
        public void Close(Type type, bool autounload = true)
        {
            var view = Get(type);

            if (null == view) return;
            if (UIState.Close == view.state) return;

            view.Close();
            if (autounload) Unload(type);
        }

        /// <summary>
        /// 关闭所有 UI 界面
        /// </summary>
        /// <param name="autounload">是否自动卸载</param>
        public void QuickClose(bool autounload = true)
        {
            var types = viewDict.Keys.ToArray();
            foreach (var type in types)
            {
                var view = Get(type);
                if (null == view) continue;
                if (view.quickclose) Close(type, autounload);
            }
        }
    }
}