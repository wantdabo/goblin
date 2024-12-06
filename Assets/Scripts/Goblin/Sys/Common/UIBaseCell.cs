using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// UI 小组件，基础类
    /// </summary>
    public abstract class UIBaseCell : UIBase<UIBaseCell>
    {
        public GameObject container { get; set; }
        private bool active { get; set; } = true;
        protected bool isActive { get { return active; } private set { active = value; } }

        /// <summary>
        /// 激活 UI
        /// </summary>
        /// <param name="status">激活状态</param>
        public void SetActive(bool status)
        {
            isActive = status;
            gameObject.SetActive(isActive);
            OnActive();
        }

        /// <summary>
        /// 设置父级 transform
        /// </summary>
        /// <param name="parent">父级 transform</param>
        public void SetParent(Transform parent)
        {
            if (null == parent) return;
            gameObject.transform.SetParent(parent, false);
            container = parent.gameObject;
        }

        public async Task<UIBaseCell> Load()
        {
            gameObject = await engine.gameres.location.LoadUIPrefabAsync(res, container.transform);
            OnLoad();
            OnBuildUI();
            OnBindEvent();

            return this;
        }

        public void Open() { OnOpen(); }

        /// <summary>
        /// UI 激活/失活回调
        /// </summary>
        protected virtual void OnActive() { }
    }
}
