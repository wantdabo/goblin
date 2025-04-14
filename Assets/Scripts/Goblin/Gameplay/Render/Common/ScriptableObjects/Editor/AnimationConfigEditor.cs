#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.ScriptableObjects.Editor
{
    [CustomEditor(typeof(AnimationConfig))]
    public class AnimationConfigEditor : UnityEditor.Editor
    {
        private ReorderableList reorderableList;

        private void OnEnable()
        {
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("animations"), true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) => GUI.Label(rect, "Animations");
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty item = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, item);
            };
        }

        public override void OnInspectorGUI()
        {
            if (null == reorderableList) return;
            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif