using ShapeDrawers.Common;
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
    [CustomPreview(typeof(EditorSphereDetectionClip))]
    public class EditorSphereDetectionClipPreview : EditorDetectionClipPreview<EditorSphereDetectionClip>
    {
        public override void OnUpdate(float time, float previousTime)
        {
            ShapeDrawer.DrawSphere(App.AssetData.cloneModel.transform.position + clip.position, clip.radius);
        }
    }
}
