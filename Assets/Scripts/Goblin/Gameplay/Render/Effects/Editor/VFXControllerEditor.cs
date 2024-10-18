#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Goblin.Gameplay.Render.Effects.Editor
{
    [CustomEditor(typeof(VFXController)), CanEditMultipleObjects]
    public class VFXControllerEditor : UnityEditor.Editor
    {
        private VFXController vfxc;
        private List<Material> mats = new();

        private void OnEnable()
        {
            vfxc = target as VFXController;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            vfxc.duration = EditorGUILayout.FloatField("持续时间：", vfxc.duration);

            EditorGUILayout.Space(10f);

            if (GUILayout.Button("初始化"))
            {
                vfxc.animators = vfxc.transform.GetComponentsInChildren<Animator>(true);
                vfxc.pss = vfxc.transform.GetComponentsInChildren<ParticleSystem>(true);

                EditorUtility.SetDirty(vfxc);
                SceneView.RepaintAll();
            }
        }
    }
}
#endif