using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.Common
{
    /// <summary>
    /// Unity3D 工具组件
    /// </summary>
    public class U3DTool : Comp<CGEngine>
    {
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
            var node = go.transform.Find(path).gameObject;
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

                if (child is T) return child as T;
                return (child as GameObject).GetComponent<T>();
            }

            return null;
        }
    }
}
