﻿using Goblin.Core;
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

                return (child as GameObject).GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// 世界坐标转UI本地坐标
        /// </summary>
        /// <param name="parentRect">父节点RectTransform</param>
        /// <param name="worldPoint">世界坐标</param>
        /// <returns>UI本地坐标</returns>
        public Vector2 WorldToUILoaclPoint(RectTransform parentRect, Vector3 worldPoint)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, engine.gameui.uicamera, out Vector2 uiPoint);

            return uiPoint;
        }

        /// <summary>
        /// 标准创建item函数
        /// </summary>
        /// <param name="需要创建的物体"></param>
        /// <param name="需要创建的物体的缓存列表"></param>
        /// <param name="当前检查数量"></param>
        /// <param name="创建位置"></param>
        public void CheckItemCreate(GameObject item, List<GameObject> itemList, int count, Transform parent)
        {
            if(itemList.Count < count)
            {
                for(int i = itemList.Count; i < count; i++)
                {
                    GameObject newItem = GameObject.Instantiate(item,parent);
                    newItem.transform.localScale = Vector3.one;
                    // 一般加在滚动视图里 位置自动调整 主要把z轴归零
                    newItem.transform.localPosition = Vector3.zero;
                    itemList.Add(newItem);
                    newItem.name = i.ToString();
                    newItem.SetActive(true);
                }
            }
            else
            {
                for(int i = 0; i < count; i++)
                {
                    itemList[i].SetActive(true);
                }
                for(int i = count; i < itemList.Count; i++)
                {
                    itemList[i].SetActive(false);
                }
            }
        }
    }
}