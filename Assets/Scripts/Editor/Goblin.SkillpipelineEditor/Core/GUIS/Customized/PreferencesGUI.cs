using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public class PreferencesGUI : ICustomized
    {
        public virtual void OnGUI()
        {
            GUILayout.BeginVertical("box");

            GUI.color = new Color(0, 0, 0, 0.3f);

            GUILayout.BeginHorizontal(Styles.headerBoxStyle);
            GUI.color = Color.white;
            GUILayout.Label($"<size=22><b>{Lan.PreferencesTitle}</b></size>");
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUILayout.BeginVertical("box");
            Prefs.magnetSnapping =
                EditorGUILayout.Toggle(new GUIContent(Lan.PreferencesMagnetSnapping, Lan.PreferencesMagnetSnappingTips),
                    Prefs.magnetSnapping);
            Prefs.scrollWheelZooms =
                EditorGUILayout.Toggle(
                    new GUIContent(Lan.PreferencesScrollWheelZooms, Lan.PreferencesScrollWheelZoomsTips),
                    Prefs.scrollWheelZooms);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            Prefs.savePath = EditorTools.GUILayoutGetFolderPath(Lan.PreferencesSavePath, Lan.PreferencesSavePathTips,
                Prefs.savePath, true);

            Prefs.autoSaveSeconds = EditorGUILayout.IntSlider(
                new GUIContent(Lan.PreferencesAutoSaveTime, Lan.PreferencesAutoSaveTimeTips), Prefs.autoSaveSeconds, 5,
                120);
            GUILayout.EndVertical();


            GUILayout.EndVertical();
        }
    }
}