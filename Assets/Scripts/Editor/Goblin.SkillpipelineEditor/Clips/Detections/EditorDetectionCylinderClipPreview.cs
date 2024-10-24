using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 圆柱体碰撞预览
    /// </summary>
    [CustomPreview(typeof(EditorDetectionCylinderClip))]
    public class EditorDetectionCylinderClipPreview : EditorDetectionClipPreview<EditorDetectionCylinderClip>
    {
        public override void OnUpdate(float time, float previousTime)
        {
            GizmosDrawer.I.DrawWireCylinder(App.AssetData.cloneModel.transform.position + clip.position, clip.radius, clip.height, new Color(153 / 255f, 214 / 255f, 83 / 255f));
        }
    }
}
