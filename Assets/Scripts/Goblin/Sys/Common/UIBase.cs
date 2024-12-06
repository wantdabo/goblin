using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Common;
using Goblin.Sys.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Goblin.Sys.Common
{
    public abstract class UIBase : Comp
    {
        /// <summary>
        /// 父对象
        /// </summary>
        public UIBase parent;

        /// <summary>
        /// UI 排序层级名
        /// </summary>
        public virtual string layerName
        {
            get
            {
                if (null == parent)
                {
                    return (this as UIBaseView).layerName;
                }

                return parent.layerName;
            }
            set { }
        }

        /// <summary>
        /// UI 排序层级序号
        /// </summary>
        public virtual int sorting
        {
            get
            {
                if (null == parent)
                {
                    return (this as UIBaseView).sorting;
                }

                return parent.sorting;
            }
            set { }
        }
    }

    /// <summary>
    /// UI 基础类
    /// </summary>
    public abstract class UIBase<T> : UIBase where T : UIBase
    {
        /// <summary>
        /// 资源名
        /// </summary>
        protected abstract string res { get; }

        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// 快速查表，此 UIBase 下的小组件，方便批量快操
        /// </summary>
        private List<UIBaseCell> cellList = new();

        /// <summary>
        /// 快速查表，所有的挂载特效
        /// </summary>
        private List<UIEffect> uieffectList = new();

        /// <summary>
        /// 添加 UI 小组件
        /// </summary>
        /// <typeparam name="UC">小组件类型</typeparam>
        /// <param name="parentNodePath">挂载的节点名字</param>
        /// <param name="active">激活状态</param>
        /// <returns>小组件</returns>
        public async Task<UC> AddUICell<UC>(string parentNodePath, bool active = true) where UC : UIBaseCell, new()
        {
            var parentNode = engine.u3dkit.GetNode<GameObject>(gameObject, parentNodePath);
            if (null == parentNode) parentNode = engine.u3dkit.SeekNode<GameObject>(gameObject, parentNodePath);

            return await AddUICell<UC>(parentNode, active);
        }

        /// <summary>
        /// 添加 UI 小组件
        /// </summary>
        /// <typeparam name="UC">小组件类型</typeparam>
        /// <param name="parentNode">挂载的节点</param>
        /// <param name="active">激活状态</param>
        /// <returns>小组件</returns>
        public async Task<UC> AddUICell<UC>(GameObject parentNode, bool active = true) where UC : UIBaseCell, new()
        {
            var comp = AddComp<UC>();
            cellList.Add(comp);
            comp.Create();

            comp.parent = this;
            comp.container = parentNode;
            await comp.Load();
            comp.SetActive(active);

            return comp;
        }

        /// <summary>
        /// 卸载小组件
        /// </summary>
        /// <param name="comp">小组件</param>
        public void RmvUICell(UIBaseCell comp)
        {
            comp.Unload();
            cellList.Remove(comp);
            comp.Destroy();
        }

        /// <summary>
        /// 卸载指定类型的所有小组件
        /// </summary>
        /// <typeparam name="UC">小组件类型</typeparam>
        public void RmvUICell<UC>() where UC : UIBaseCell
        {
            for (int i = cellList.Count - 1; i >= 0; i--) if (cellList[i] is UC) RmvUICell(cellList[i]);
        }

        /// <summary>
        /// 卸载 UI
        /// </summary>
        public void Unload()
        {
            for (int i = cellList.Count - 1; i >= 0; i--) RmvUICell(cellList[i]);
            OnUnload();
            GameObject.Destroy(gameObject);
        }

        /// <summary>
        /// UI 加载回调
        /// </summary>
        protected virtual void OnLoad()
        {
        }

        /// <summary>
        /// UI 卸载回调
        /// </summary>
        protected virtual void OnUnload()
        {
        }

        /// <summary>
        /// UI 打开回调
        /// </summary>
        protected virtual void OnOpen()
        {
            foreach (var cell in cellList) cell.Open();
            foreach (var eff in uieffectList) eff.Sorting(layerName, sorting);
        }

        /// <summary>
        /// 关闭 UI
        /// </summary>
        public void Close()
        {
            foreach (var cell in cellList) cell.Close();
            OnClose();
        }

        /// <summary>
        /// UI 关闭回调
        /// </summary>
        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// 构建 UI 回调，用于添加小组件，获取并缓存指定的动态 Unity3D 组件
        /// </summary>
        protected virtual void OnBuildUI()
        {
        }

        /// <summary>
        /// 绑定事件回调可以集中写在这里
        /// </summary>
        protected virtual void OnBindEvent()
        {
        }

        /// <summary>
        /// 注册 UI 事件
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="action">回调</param>
        /// <param name="eventType">事件类型，默认点击</param>
        protected void AddUIEventListener(string nodeName, Action<PointerEventData> action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            AddUIEventListener(engine.u3dkit.SeekNode<GameObject>(gameObject, nodeName), action, eventType);
        }

        /// <summary>
        /// 注册 UI 事件
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="action">回调</param>
        /// <param name="eventType">事件类型，默认点击</param>
        protected void AddUIEventListener(GameObject node, Action<PointerEventData> action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            var listener = engine.u3dkit.GetNode<UIEventListener>(node);
            if (null == listener) listener = node.AddComponent<UIEventListener>();

            listener.AddListener(eventType, action);
        }

        /// <summary>
        /// 添加特效
        /// </summary>
        /// <param name="nodename">节点名</param>
        /// <param name="res">特效名</param>
        /// <returns>特效</returns>
        protected UIEffect AddUIEffect(string nodename, string res)
        {
            return AddUIEffect(engine.u3dkit.SeekNode<GameObject>(gameObject, nodename), res);
        }

        /// <summary>
        /// 添加特效
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="res">特效名</param>
        /// <returns>特效</returns>
        protected UIEffect AddUIEffect(GameObject node, string res)
        {
            var eff = AddComp<UIEffect>();
            eff.Load(node, res);
            eff.Sorting(layerName, sorting);
            uieffectList.Add(eff);

            return eff;
        }

        /// <summary>
        /// 设置 Image 的 Sprite
        /// </summary>
        /// <param name="image">Image 组件</param>
        /// <param name="res">资源名</param>
        /// <param name="nativesize">尺寸自适应</param>
        protected async void SetSprite(Image image, string res, bool nativesize = true)
        {
            var sprite = await engine.gameres.location.LoadSpriteAsync(res);
            if (null == sprite) return;

            image.sprite = sprite;
            if (nativesize) image.SetNativeSize();
        }

        /// <summary>
        /// 设置 RawImage 的 Texture
        /// </summary>
        /// <param name="rawimage">RawImage 组件</param>
        /// <param name="res">资源名</param>
        /// <param name="nativesize">尺寸自适应</param>
        protected async void SetSprite(RawImage rawimage, string res, bool nativesize = true)
        {
            var sprite = await engine.gameres.location.LoadSpriteAsync(res);
            if (null == sprite) return;

            rawimage.texture = sprite.texture;
            if (nativesize) rawimage.SetNativeSize();
        }
    }
}
