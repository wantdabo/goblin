using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoblinFramework.Client.UI.Common
{
    public enum UIEventEnum
    {
        BeginDrag,
        Drag,
        EndDrag,
        PointerClick,
        PointerDown,
        PointerUp,
        PointerEnter,
        PointerExit,
    }

    [DisallowMultipleComponent]
    public class UIEventListener : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector]
        private ScrollRect scrollRect;

        [HideInInspector]
        private Dictionary<UIEventEnum, List<Action<PointerEventData>>> eventDic = new Dictionary<UIEventEnum, List<Action<PointerEventData>>>();

        private void OnEnable()
        {
            scrollRect = FindScrollRect(transform);
        }

        private ScrollRect FindScrollRect(Transform trans)
        {
            if (null == trans) return null;
            if (null != trans.GetComponent<Canvas>()) return GetComponent<ScrollRect>();
            if (null != trans.GetComponent<ScrollRect>()) return trans.GetComponent<ScrollRect>();

            return FindScrollRect(trans.parent);
        }

        public void AddListener(UIEventEnum eventType, Action<PointerEventData> action)
        {
            List<Action<PointerEventData>> eventList;
            if (eventDic.TryGetValue(eventType, out eventList) == false)
                eventList = new List<Action<PointerEventData>>();

            eventList.Add(action);
            eventDic.Add(eventType, eventList);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            scrollRect?.OnBeginDrag(eventData);
            DispatchEvent(UIEventEnum.BeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            scrollRect?.OnDrag(eventData);
            DispatchEvent(UIEventEnum.Drag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            scrollRect?.OnEndDrag(eventData);
            DispatchEvent(UIEventEnum.EndDrag, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            DispatchEvent(UIEventEnum.PointerClick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            DispatchEvent(UIEventEnum.PointerDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DispatchEvent(UIEventEnum.PointerUp, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DispatchEvent(UIEventEnum.PointerEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DispatchEvent(UIEventEnum.PointerExit, eventData);
        }

        private void DispatchEvent(UIEventEnum eventType, PointerEventData eventData)
        {
            if (false == eventDic.ContainsKey(eventType)) return;
            foreach (var item in eventDic[eventType]) item.Invoke(eventData);
        }

    }
}