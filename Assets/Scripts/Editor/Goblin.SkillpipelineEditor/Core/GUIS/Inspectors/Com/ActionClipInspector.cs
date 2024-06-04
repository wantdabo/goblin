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
            ShowCommonInspector();
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
                GUILayout.Label("IN", GUILayout.Width(30));
                _in *= Prefs.frameRate;
                _in = EditorGUILayout.DelayedIntField((int)_in, GUILayout.Width(80));
                _in *= (1f / Prefs.frameRate);

                GUILayout.FlexibleSpace();
                GUILayout.Label("◄");
                _length *= Prefs.frameRate;
                _length = EditorGUILayout.DelayedIntField((int)_length, GUILayout.Width(80));
                _length *= (1f / Prefs.frameRate);
                GUILayout.Label("►");
                GUILayout.FlexibleSpace();

                GUILayout.Label("OUT", GUILayout.Width(30));
                _out *= Prefs.frameRate;
                _out = EditorGUILayout.DelayedIntField((int)_out, GUILayout.Width(80));
                _out *= (1f / Prefs.frameRate);
            }

            GUILayout.EndHorizontal();

            if (canScale)
            {
                if (_in >= action.Parent.StartTime && _out <= action.Parent.EndTime)
                {
                    if (_out > _in)
                    {
                        EditorGUILayout.MinMaxSlider(ref _in, ref _out, previousTime, nextTime);
                    }
                    else
                    {
                        _in = EditorGUILayout.Slider(_in, previousTime, nextTime);
                        _out = _in;
                    }
                }
            }
            else
            {
                GUILayout.Label("IN", GUILayout.Width(30));
                _in = EditorGUILayout.Slider(_in, 0, action.Parent.EndTime);
                _out = _in;
            }


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