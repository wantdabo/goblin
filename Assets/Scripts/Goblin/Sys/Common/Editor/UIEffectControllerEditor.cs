#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Goblin.Sys.Common
{
    [CustomEditor(typeof(UIEffectController))]
    public class UIEffectControllerEditor : Editor
    {
        private UIEffectController uiec;

        private void OnEnable()
        {
            uiec = target as UIEffectController;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            uiec.duration = EditorGUILayout.FloatField("持续时间：", uiec.duration);

            EditorGUILayout.Space(10f);

            if (GUILayout.Button("初始化"))
            {
                uiec.pss = uiec.transform.GetComponentsInChildren<ParticleSystem>(true);
                uiec.renders = uiec.transform.GetComponentsInChildren<MeshRenderer>(true);
                uiec.animators = uiec.transform.GetComponentsInChildren<Animator>(true);

                EditorUtility.SetDirty(uiec);
                AssetDatabase.SaveAssets();
                SceneView.RepaintAll();
            }
        }
    }
}
#endif