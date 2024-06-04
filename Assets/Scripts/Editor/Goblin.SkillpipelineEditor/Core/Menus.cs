using UnityEditor;

namespace Goblin.SkillPipelineEditor
{
    public static class Menus
    {
        [MenuItem("Tools/SkillPipeline Workspaces", false, 0)]
        public static void OpenDirectorWindow()
        {
            ActionEditorWindow.ShowWindow();
        }
    }
}