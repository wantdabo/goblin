using GoblinFramework.Client.Common;
using GoblinFramework.Client.UI.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoblinFramework.Client.UI.Base
{
    /// <summary>
    /// UI 基础类
    /// </summary>
    public abstract class UIBase : CComp
    {
        protected abstract string UIRes { get; }

        public GameObject gameObject;

        /// <summary>
        /// 获得一些 UI 小组件
        /// </summary>
        /// <typeparam name="T">部件类型</typeparam>
        /// <returns></returns>
        public List<T> GetUICells<T>() where T : UIBaseCell
        {
            return GetComps<T>();
        }

        /// <summary>
        /// 快速查表，此 UIBase 下的小组件，方便批量快操
        /// </summary>
        private List<UIBaseCell> cellList = new List<UIBaseCell>();

        /// <summary>
        /// 添加 UI 小组件
        /// </summary>
        /// <typeparam name="T">小组件类型</typeparam>
        /// <param name="parentNodePath">挂载的节点名字</param>
        /// <param name="active">激活状态</param>
        /// <returns>小组件</returns>
        public T AddUICell<T>(string parentNodePath, bool active = true) where T : UIBaseCell, new()
        {
            var parentNode = Engine.U3D.GetNode<GameObject>(gameObject, parentNodePath);
            if (null == parentNode) Engine.U3D.SeekNode<GameObject>(gameObject, parentNodePath);

            return AddUICell<T>(parentNode, active);
        }

        /// <summary>
        /// 添加 UI 小组件
        /// </summary>
        /// <typeparam name="T">小组件类型</typeparam>
        /// <param name="parentNodePath">挂载的节点</param>
        /// <param name="active">激活状态</param>
        /// <returns>小组件</returns>
        public T AddUICell<T>(GameObject parentNode, bool active = true) where T : UIBaseCell, new()
        {
            var comp = base.AddComp<T>();
            cellList.Add(comp);

            comp.Container = parentNode;
            comp.Load();
            comp.SetActive(active);

            return comp;
        }

        /// <summary>
        /// 卸载小组件
        /// </summary>
        /// <param name="comp">小组件</param>
        public void RmvUICell(UIBaseCell comp)
        {
            cellList.Remove(comp);

            comp.Unload();
            base.RmvComp(comp);
        }

        /// <summary>
        /// 卸载指定类型的所有小组件
        /// </summary>
        /// <typeparam name="T">小组件类型</typeparam>
        public void RmvUICell<T>() where T : UIBaseCell
        {
            for (int i = cellList.Count - 1; i >= 0; i--)
                if (cellList[i] is T) base.RmvComp(cellList[i]);
        }

        /// <summary>
        /// 加载 UI
        /// </summary>
        public virtual void Load()
        {
            OnLoad();
        }

        /// <summary>
        /// 卸载 UI
        /// </summary>
        public virtual void Unload()
        {
            OnUnload();
            GameObject.Destroy(gameObject);
        }

        /// <summary>
        /// 打开 UI
        /// </summary>
        public virtual void Open()
        {
            foreach (var cell in cellList) cell.Open();
            OnOpen();
        }

        /// <summary>
        /// 关闭 UI
        /// </summary>
        public virtual void Close()
        {
            foreach (var cell in cellList) cell.Close();
            OnClose();
        }

        /// <summary>
        /// UI 加载回调
        /// </summary>
        protected virtual void OnLoad() { base.OnCreate(); }

        /// <summary>
        /// UI 卸载回调
        /// </summary>
        protected virtual void OnUnload() { base.OnDestroy(); }

        /// <summary>
        /// UI 打开回调
        /// </summary>
        protected virtual void OnOpen() { }

        /// <summary>
        /// UI 关闭回调
        /// </summary>
        protected virtual void OnClose() { }

        /// <summary>
        /// 构建 UI 回调，用于添加小组件，获取并缓存指定的动态 Unity3D 组件
        /// </summary>
        protected virtual void OnBuildUI() { }

        /// <summary>
        /// 绑定事件回调可以集中写在这里
        /// </summary>
        protected virtual void OnBindEvent() { }

        /// <summary>
        /// 注册 UI 事件
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="action">回调</param>
        /// <param name="eventType">事件类型，默认点击</param>
        public void AddUIEventListener(string nodeName, Action<PointerEventData> action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            AddUIEventListener(Engine.U3D.SeekNode<GameObject>(gameObject, nodeName), action, eventType);
        }

        /// <summary>
        /// 注册 UI 事件
        /// </summary>
        /// <param name="nodeName">节点</param>
        /// <param name="action">回调</param>
        /// <param name="eventType">事件类型，默认点击</param>
        public void AddUIEventListener(GameObject node, Action<PointerEventData> action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            if (null == node) throw new Exception($"node can't be null, plz check.");

            var listener = Engine.U3D.GetNode<UIEventListener>(node);
            if (null == listener) listener = node.AddComponent<UIEventListener>();

            listener.AddListener(eventType, action);
        }
    }
}
