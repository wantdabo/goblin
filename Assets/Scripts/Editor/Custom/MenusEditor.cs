using Goblin.SkillPipelineEditor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Goblin.Custom
{
    public static class MenusEditor
    {
        [MenuItem("工具/GOBLIN 技能管线编辑器", false, 1)]
        public static void OpenDirectorWindow()
        {
            ActionEditorWindow.ShowWindow();
        }

        [MenuItem("工具/GOBLIN 运行时", false, 0)]
        public static void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/GameRes/Scene/Main.unity");
        }
    }
}
