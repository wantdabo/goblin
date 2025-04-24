#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.Editor
{
    /// <summary>
    /// 前置混合动画信息编辑
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimationInfo))]
    public class AnimationInfoEditor : PropertyDrawer
    {
        private ReorderableList reorderableList;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight * 3;
            SerializedProperty mixAnimations = property.FindPropertyRelative("mixanimations");

            InitializeReorderableList(mixAnimations);

            totalHeight += reorderableList.GetHeight();
            totalHeight += EditorGUIUtility.standardVerticalSpacing * 3;

            return totalHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(rect, label, property))
            {
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                Rect currentRect = rect;
                currentRect.height = lineHeight;

                // 绘制基本属性
                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("state"), new GUIContent("State"));
                currentRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("name"), new GUIContent("Name"));
                currentRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("mixduration"), new GUIContent("Mixduration"));
                currentRect.y += lineHeight + spacing;

                // 绘制 mixanimations 的 ReorderableList
                SerializedProperty mixAnimations = property.FindPropertyRelative("mixanimations");
                if (reorderableList == null)
                {
                    InitializeReorderableList(mixAnimations);
                }

                currentRect.height = reorderableList.GetHeight();
                reorderableList.DoList(currentRect);
            }
        }

        private void InitializeReorderableList(SerializedProperty mixAnimations)
        {
            reorderableList = new ReorderableList(mixAnimations.serializedObject, mixAnimations, true, true, true, true);

            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Mix Animations");
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = mixAnimations.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            reorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = mixAnimations.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
            };
        }
    }
}
#endif