using UnityEditor;

namespace Goblin.SkillPipelineEditor
{
    public static class Menus
    {
        [MenuItem("工具/GOBLIN 技能管线编辑器", false, 0)]
        public static void OpenDirectorWindow()
        {
            ActionEditorWindow.ShowWindow();
        }
    }
}