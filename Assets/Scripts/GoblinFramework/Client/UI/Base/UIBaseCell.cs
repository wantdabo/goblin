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

        public GameObject Container { get { return container; } set { container = value; } }

        public void Load() 
        {
            gameObject = Engine.GameRes.Location.LoadUIPrefabSync(UIRes, Container.transform);
            OnBuildUI();
            OnBindEvent();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            gameObject.SetActive(IsActive);
        }

        protected override void OnClose()
        {
            base.OnClose();
            gameObject.SetActive(IsActive);
        }
    }
}
