using GoblinFramework.Render.Common;
using GoblinFramework.Render.UI.Base;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Render.UI
{
    /// <summary>
    /// Game-UI 游戏 UI 组件
    /// </summary>
    public class GameUI : RComp
    {
        #region 枚举
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
        #endregion

        public GameObject UIRoot;
        public Camera UICamrea;

        protected override void OnCreate()
        {
            base.OnCreate();
            UIRoot = GameObject.Find("UI/UIRoot");
            UICamrea = GameObject.Find("UI/UICamera").GetComponent<Camera>();

            // 批量生成 UILayer
            foreach (var name in Enum.GetNames(typeof(UILayer))) if (Enum.TryParse(name, out UILayer result)) GenUILayerNode(result);
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

        /// <summary>
        /// UI 当前最顶 Sorting
        /// </summary>
        private const int sorting = 0;

        /// <summary>
        /// UI 之间 Sorting 间距
        /// </summary>
        private const int sortingSpacing = 10;
        public T OpenView<T>() where T : UIBaseView, new()
        {
            var view = AddComp<T>();
            view.Sorting = sorting + sortingSpacing;
            view.Open();

            return view;
        }

        public void CloseView(UIBaseView view) 
        {
            view.Close();
            RmvComp(view);
        }
    }
}
