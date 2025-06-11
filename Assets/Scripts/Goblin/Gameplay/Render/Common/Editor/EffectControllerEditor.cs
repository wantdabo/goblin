using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common.Editor
{
    /// <summary>
    /// EffectController 编辑器
    /// </summary>
    [CustomEditor(typeof(EffectController)), CanEditMultipleObjects]
    public class EffectControllerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// 特效控制器
        /// </summary>
        private EffectController controller { get; set; }

        private void OnEnable()
        {
            controller = target as EffectController;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10f);

            if (GUILayout.Button("初始化"))
            {
                controller.animators = controller.transform.GetComponentsInChildren<Animator>(true);
                controller.pss = controller.transform.GetComponentsInChildren<ParticleSystem>(true);

                EditorUtility.SetDirty(controller);
                SceneView.RepaintAll();
            }
        }
    }
}