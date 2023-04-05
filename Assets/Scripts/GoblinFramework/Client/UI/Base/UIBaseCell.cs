using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.UI.Base
{
    /// <summary>
    /// UI 小组件，基础类
    /// </summary>
    public abstract class UIBaseCell : UIBase
    {
        private GameObject container;

        private bool active = true;
        protected bool IsActive { get { return active; } private set { active = value; } }

        public GameObject Container { get { return container; } set { container = value; } }

        /// <summary>
        /// 激活 UI
        /// </summary>
        /// <param name="status">激活状态</param>
        public void SetActive(bool status)
        {
            IsActive = status;
            gameObject.SetActive(IsActive);
            OnActive();
        }

        public override void Load()
        {
            gameObject = Engine.GameRes.Location.LoadUIPrefabSync(UIRes, Container.transform);
            OnBuildUI();
            OnBindEvent();
            base.Load();
        }

        /// <summary>
        /// UI 激活/失活回调
        /// </summary>
        protected virtual void OnActive() { }
    }
}
