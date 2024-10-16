using UnityEditor;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public abstract class ActionClipInspector<T> : ActionClipInspector where T : ActionClip
    {
        protected T action => (T)target;
    }

    [CustomInspectors(typeof(ActionClip), true)]
    public class ActionClipInspector : InspectorsBase
    {
        private ActionClip action => (ActionClip)target;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            ShowCommonInspector();
            if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
        }

        protected void ShowCommonInspector(bool showBaseInspector = true)
        {
            ShowErrors();
            ShowInOutControls();
            if (showBaseInspector)
            {
                base.OnInspectorGUI();
            }
        }

        void ShowErrors()
        {
            if (action.isValid) return;
            EditorGUILayout.HelpBox("该剪辑无效。 请确保设置了所需的参数。",
                MessageType.Error);
            GUILayout.Space(5);
        }

        void ShowInOutControls()
        {
            var previousClip = action.GetPreviousSibling();
            var previousTime = previousClip != null ? previousClip.EndTime : action.Parent.StartTime;

            var nextClip = action.GetNextSibling();
            var nextTime = nextClip != null ? nextClip.StartTime : action.Parent.EndTime;

            var canScale = action.CanScale();

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();

            var _in = action.StartTime;
            var _length = action.Length;
            var _out = action.EndTime;

            if (canScale)
            {
                GUILayout.Label("IN");
                _in *= Prefs.frameRate;
                _in = EditorGUILayout.DelayedIntField((int)_in);
                _in *= (1f / Prefs.frameRate);

                GUILayout.Label("OUT", GUILayout.Width(30));
                _out *= Prefs.frameRate;
                _out = EditorGUILayout.DelayedIntField((int)_out);
                _out *= (1f / Prefs.frameRate);
                
                GUILayout.Label("LEN", GUILayout.Width(30));
                _length *= Prefs.frameRate;
                _length = EditorGUILayout.DelayedIntField((int)_length);
                _length *= (1f / Prefs.frameRate);
            }

            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                if (_length != action.Length)
                {
                    _out = _in + _length;
                }

                _in = Mathf.Round(_in / Prefs.snapInterval) * Prefs.snapInterval;
                _out = Mathf.Round(_out / Prefs.snapInterval) * Prefs.snapInterval;

                _in = Mathf.Clamp(_in, previousTime, _out);
                _out = Mathf.Clamp(_out, _in, nextClip != null ? nextTime : float.PositiveInfinity);

                action.StartTime = _in;
                action.EndTime = _out;
            }

            if (_in > action.Parent.EndTime)
            {
                EditorGUILayout.HelpBox(Lan.OverflowInvalid, MessageType.Warning);
            }
            else
            {
                if (_out > action.Parent.EndTime)
                {
                    EditorGUILayout.HelpBox(Lan.EndTimeOverflowInvalid, MessageType.Warning);
                }
            }

            if (_out < action.Parent.StartTime)
            {
                EditorGUILayout.HelpBox(Lan.OverflowInvalid, MessageType.Warning);
            }
            else
            {
                if (_in < action.Parent.StartTime)
                {
                    EditorGUILayout.HelpBox(Lan.StartTimeOverflowInvalid, MessageType.Warning);
                }
            }

            GUILayout.EndVertical();
        }
    }
}