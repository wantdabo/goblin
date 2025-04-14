using UnityEditor;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.ScriptableObjects.Editor
{
    [CustomPropertyDrawer(typeof(AnimationInfo))]
    public class AnimationInfoEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
            }
        }
    }
}