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
    /// <summary>
    /// UI 基础类
    /// </summary>
    public abstract class UIBase : Comp<CGEngineComp>
    {
        protected abstract string UIRes { get; }

        private bool active = true;
        protected bool IsActive { get { return active; } private set { active = value; } }

        public GameObject go;

        public void SetActive(bool status)
        {
            IsActive = status;
            go.SetActive(IsActive);
            OnActive();
        }

        public List<T> GetUICells<T>() where T : UIBaseCell
        {
            return GetComp<T>();
        }

        public T AddUICell<T>(string parentNodePath, bool active = true) where T : UIBaseCell, new()
        {
            var parentNode = Engine.U3D.GetNode<GameObject>(go, parentNodePath);
            if (null == parentNode) Engine.U3D.SeekNode<GameObject>(go, parentNodePath);

            return AddUICell<T>(parentNode, active);
        }

        private List<UIBaseCell> cellList = new List<UIBaseCell>();
        public T AddUICell<T>(GameObject parentNode, bool active = true) where T : UIBaseCell, new()
        {
            var comp = AddComp<T>();
            cellList.Add(comp);

            comp.Container = parentNode;
            comp.Load();
            comp.SetActive(active);

            return comp;
        }

        public void RmvUICell(UIBaseCell comp)
        {
            RmvComp(comp);
            cellList.Remove(comp);
        }

        public void RmvUICell<T>() where T : UIBaseCell
        {
            RmvComp<T>();
            for (int i = cellList.Count - 1; i >= 0; i--) cellList.RemoveAt(i);
        }

        protected virtual void OnOpen()
        {
            foreach (var cell in cellList) cell.OnOpen();
        }

        protected virtual void OnClose()
        {
            foreach (var cell in cellList) cell.OnClose();
        }

        protected virtual void OnBuildUI() { }
        protected virtual void OnBindEvent() { }
        protected virtual void OnActive() { }
    }
}
