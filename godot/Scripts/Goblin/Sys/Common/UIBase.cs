using Goblin.Core;
using Goblin.Sys.Common;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goblin.Sys.Common
{
    public enum UIEventEnum { BeginDrag, Drag, EndDrag, PointerClick, PointerDown, PointerUp, PointerEnter, PointerExit }

    public abstract class UIBase : Comp
    {
        public UIBase parent;

        public virtual string layerName
        {
            get => null == parent ? (this as UIBaseView)?.layerName : parent.layerName;
            set { }
        }

        public virtual int sorting
        {
            get => null == parent ? (this as UIBaseView)?.sorting ?? 0 : parent.sorting;
            set { }
        }
    }

    public abstract class UIBase<T> : UIBase where T : UIBase
    {
        protected abstract string res { get; }

        public Control node { get; set; }

        private List<UIBaseCell> cellList = new();
        private List<UIEffect> uieffectList = new();

        public async Task<UC> AddUICell<UC>(string nodePath, bool active = true) where UC : UIBaseCell, new()
        {
            var parentNode = node?.FindChild(nodePath, true, false) as Control ?? node;
            return await AddUICell<UC>(parentNode, active);
        }

        public async Task<UC> AddUICell<UC>(Control parentNode, bool active = true) where UC : UIBaseCell, new()
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

        public void RmvUICell(UIBaseCell comp)
        {
            comp.Unload(); cellList.Remove(comp); comp.Destroy();
        }

        public void RmvUICell<UC>() where UC : UIBaseCell
        {
            for (int i = cellList.Count - 1; i >= 0; i--) if (cellList[i] is UC) RmvUICell(cellList[i]);
        }

        public void Unload()
        {
            for (int i = cellList.Count - 1; i >= 0; i--) RmvUICell(cellList[i]);
            OnUnload();
            node?.QueueFree();
            node = null;
        }

        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }

        protected virtual void OnOpen()
        {
            foreach (var cell in cellList) cell.Open();
        }

        public void Close()
        {
            foreach (var cell in cellList) cell.Close();
            OnClose();
        }

        protected virtual void OnClose() { }
        protected virtual void OnBuildUI() { }
        protected virtual void OnBindEvent() { }

        protected void AddUIEventListener(string nodeName, Action action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            var target = node?.FindChild(nodeName, true, false) as Control;
            if (null == target) return;
            AddUIEventListener(target, action, eventType);
        }

        protected void AddUIEventListener(Control target, Action action, UIEventEnum eventType = UIEventEnum.PointerClick)
        {
            if (null == target) return;
            if (eventType == UIEventEnum.PointerClick && target is Button btn)
                btn.Pressed += action;
            else if (eventType == UIEventEnum.PointerClick)
                target.GuiInput += (e) => { if (e is InputEventMouseButton mb && mb.Pressed) action(); };
        }

        protected UIEffect AddUIEffect(string nodeName, string res)
        {
            var target = node?.FindChild(nodeName, true, false) as Control ?? node;
            return AddUIEffect(target, res);
        }

        protected UIEffect AddUIEffect(Control target, string res)
        {
            var eff = AddComp<UIEffect>();
            eff.Load(target, res);
            uieffectList.Add(eff);
            return eff;
        }
    }
}
