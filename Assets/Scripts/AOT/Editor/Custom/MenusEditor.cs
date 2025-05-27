using UnityEditor;
using UnityEditor.SceneManagement;

namespace Goblin.Custom
{
    public static class MenusEditor
    {
        [MenuItem("工具/GOBLIN 管线编辑器", false, 1)]
        public static void OpenPipelineScene()
        {
            EditorSceneManager.OpenScene("Assets/UERes/Gameplay/Workspaces/PipelineScene.unity");
        }

        [MenuItem("工具/GOBLIN 运行时", false, 0)]
        public static void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/GameRes/Scene/Main.unity");
        }
    }
}
