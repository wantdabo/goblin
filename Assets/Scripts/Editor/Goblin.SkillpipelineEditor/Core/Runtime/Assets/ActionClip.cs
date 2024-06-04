#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    [Serializable]
    [Attachable(typeof(Track))]
    public abstract class ActionClip : DirectableAsset
    {
        [SerializeField] private float startTime;

        public virtual Track Parent
        {
            get => (Track)_parent;
            set => _parent = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public override float StartTime
        {
            get => startTime;
            set
            {
                if (Math.Abs(startTime - value) > 0.0001f)
                {
                    startTime = Mathf.Max(value, 0);
                }
            }
        }

        public override float EndTime
        {
            get => StartTime + Length;
            set
            {
                if (Math.Abs(StartTime + Length - value) > 0.0001f) //if (StartTime + length != value)
                {
                    Length = Mathf.Max(value - StartTime, 0);
                }
            }
        }

        public override bool IsActive => Parent != null ? Parent.IsActive : false;
        public override bool IsCollapsed => Parent != null && Parent.IsCollapsed;

        public override bool IsLocked => Parent != null && Parent.IsLocked;

        public virtual float Length
        {
            get => 0;
            set { }
        }
        
        public virtual string info
        {
            get
            {
                var nameAtt = this.GetType().RTGetAttribute<NameAttribute>(true);
                if (nameAtt != null)
                {
                    return nameAtt.name;
                }

                return this.GetType().Name.SplitCamelCase();
            }
        }

        public virtual bool isValid => false;

        

        public ActionClip GetNextClip()
        {
            return this.GetNextSibling<ActionClip>();
        }

        public void TryMatchSubClipLength()
        {
            if (this is ISubClipContainable)
            {
                Length = ((ISubClipContainable)this).SubClipLength / ((ISubClipContainable)this).SubClipSpeed;
            }
        }

        #region Unity Editor

#if UNITY_EDITOR

        public void ShowClipGUI(Rect rect)
        {
            OnClipGUI(rect);
        }

        public void ShowClipGUIExternal(Rect left, Rect right)
        {
            OnClipGUIExternal(left, right);
        }

        protected virtual void OnClipGUI(Rect rect)
        {
        }

        protected virtual void OnClipGUIExternal(Rect left, Rect right)
        {
        }

#endif
        #endregion
    }
}
#endif