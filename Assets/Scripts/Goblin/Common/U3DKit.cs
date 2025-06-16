using Goblin.Core;
using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Common
{
    /// <summary>
    /// Unity3D 工具组件
    /// </summary>
    public class U3DKit : Comp
    {
        /// <summary>
        /// 输入绑定
        /// </summary>
        public Gamepad gamepad { get; private set; }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = new Gamepad();
            gamepad.Enable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gamepad.Dispose();
        }

        /// <summary>
        /// 获取 Unity3D 节点/组件
        /// </summary>
        /// <typeparam name="T">Unity 的类型，GameObject 或 Component</typeparam>
        /// <param name="go">节点</param>
        /// <returns>GameObject/Component</returns>
        public T GetNode<T>(GameObject go) where T : UnityEngine.Object
        {
            if (go is T) return go as T;

            return go.GetComponent<T>();
        }

        /// <summary>
        /// 精准查找 Unity3D 节点/组件
        /// </summary>
        /// <typeparam name="T">Unity 的类型，GameObject 或 Component</typeparam>
        /// <param name="go">根节点</param>
        /// <param name="path">节点路径</param>
        /// <returns>GameObject/Component</returns>
        public T GetNode<T>(GameObject go, string path) where T : UnityEngine.Object
        {
            var node = go.transform.Find(path)?.gameObject;

            if (null == node) return null;

            if (node is T) return node as T;

            return node.GetComponent<T>();
        }

        /// <summary>
        /// 模糊查找 Unity3D 节点/组件
        /// </summary>
        /// <typeparam name="T">Unity 的类型，GameObject 或 Component</typeparam>
        /// <param name="go">预制体根节点</param>
        /// <param name="nodeName">匹配节点名</param>
        /// <returns>GameObject/Component</returns>
        public T SeekNode<T>(GameObject go, string nodeName) where T : UnityEngine.Object
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var node = go.transform.GetChild(i).gameObject;
                if (node.name.Equals(nodeName))
                {
                    if (node is T) return node as T;

                    return node.GetComponent<T>();
                }

                var child = SeekNode<T>(node, nodeName);
                if (null == child) continue;

                if (child is T) return child;
            }

            return null;
        }

        /// <summary>
        /// 世界坐标转 UI 本地坐标
        /// </summary>
        /// <param name="parentRect">父节点 RectTransform</param>
        /// <param name="worldPoint">世界坐标</param>
        /// <returns>UI 本地坐标</returns>
        public Vector2 WorldToUILoaclPoint(RectTransform parentRect, Vector3 worldPoint)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, engine.gameui.uicamera, out Vector2 uiPoint);

            return uiPoint;
        }
    }
}