using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.UI
{
    public class GameUI : ClientComp
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

        public GameObject UINode;
        public GameObject UIRoot;
        public Camera UICamrea;

        protected override void OnCreate()
        {
            base.OnCreate();
            UINode = GameObject.Find("UI");
            UIRoot = GameObject.Find("UI/UIRoot");
            UICamrea = GameObject.Find("UI/UICamera")?.GetComponent<Camera>();

            // 批量生成 UILayer
            foreach (var name in Enum.GetNames(typeof(UILayer))) if (Enum.TryParse(name, out UILayer result)) GenUILayerNode(result);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private Dictionary<UILayer, GameObject> uiLayerNodeDict = new Dictionary<UILayer, GameObject>();
        private Dictionary<UILayer, Stack<UIBaseView>> viewStackDict = new Dictionary<UILayer, Stack<UIBaseView>>();
        private void GenUILayerNode(UILayer layer)
        {
            var layerNode = new GameObject(layer.ToString());
            uiLayerNodeDict.Add(layer, layerNode);
            layerNode.transform.SetParent(UIRoot.transform, true);

            // 设定 RectTransform
            var rtf = layerNode.AddComponent<RectTransform>();
            rtf.sizeDelta = Vector2.zero;
            rtf.anchorMin = Vector2.zero;
            rtf.anchorMax = Vector2.one;
            rtf.localPosition = Vector3.zero;
            rtf.localScale = Vector3.one;

            // 设定 CanvasGroup
            var cg = layerNode.AddComponent<Canvas>();
            cg.overrideSorting = true;
            cg.sortingLayerName = layer.ToString();
        }

        private int sortingSpacing = 10;
        public T OpenView<T>() where T : UIBaseView, new()
        {
            var view = AddComp<T>();

            // 设置层级
            if (uiLayerNodeDict.TryGetValue(view.UILayer, out var node)) view.gameObject.transform.SetParent(node.transform, true);

            if (viewStackDict.TryGetValue(view.UILayer, out var stack))
            {
                var topView = stack.Peek();
                view.Sorting = (topView ?? view).Sorting + sortingSpacing;
                stack.Push(view);
            }

            return view;
        }

        public void CloseView(UIBaseView view) 
        {
            if (false == viewStackDict.TryGetValue(view.UILayer, out var stack)) return;
        }
    }
}
