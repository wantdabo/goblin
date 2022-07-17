using GoblinFramework.Client.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Client.UI.Base
{
    public abstract class UIBase : Comp
    {
        public GameObject go;

        protected abstract string UIRes { get; }

        public List<UIBaseCell> GetUICells<T>() where T : UIBaseCell
        {
            return GetComp<T>() as List<UIBaseCell>;
        }

        public UIBaseCell AddUICell<T>(string node) where T : UIBaseCell, new()
        {
            return AddUICell<T>(go.transform.Find(node).gameObject);
        }

        public UIBaseCell AddUICell<T>(GameObject node) where T : UIBaseCell, new()
        {
            var comp = AddComp<T>();
            comp.Container = node;

            return comp;
        }

        public void RmvUICell(UIBaseCell comp)
        {
            RmvComp(comp);
        }

        public void RmvUICell<T>() where T : UIBaseCell
        {
            RmvComp<T>();
        }

        protected virtual void OnBuildUI() { }
        protected virtual void OnBindEvent() { }
        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
    }
}
