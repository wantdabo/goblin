using ShapeDrawers.Common;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 子弹预览
    /// </summary>
    [CustomPreview(typeof(EditorBulletEventClip))]
    public class EditorBulletEventClipPreview : PreviewBase<EditorBulletEventClip>
    {
        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;
            if (time > clip.GetLength()) return;
            if (0 == time && previousTime > 0) return;
            
            ShapeDrawer.DrawBox(App.AssetData.cloneModel.transform.position + clip.position, Vector3.one * 0.5f, Quaternion.identity, Color.blue);
        }
    }
}
