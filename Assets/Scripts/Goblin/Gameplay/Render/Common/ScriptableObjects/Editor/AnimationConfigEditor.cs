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
            
            // 使用回调函数动态计算每个元素高度
            reorderableList.elementHeightCallback = (int index) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing * 2;
            };
            
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty item = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUI.GetPropertyHeight(item, true);
                EditorGUI.PropertyField(rect, item, true);
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