using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace GoblinFramework.Client.UI
{
    public abstract class UIBase : ClientComp
    {
        protected abstract string UIRes { get; }
        public GameObject gameObject;

        public List<UIBaseCell> GetUICells<T>() where T : UIBaseCell
        {
            return GetComp<T>() as List<UIBaseCell>;
        }

        public UIBaseCell AddUICell<T>(string node) where T : UIBaseCell, new()
        {
            return AddUICell<T>(gameObject.transform.Find(node).gameObject);
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
