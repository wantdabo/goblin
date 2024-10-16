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
            GizmosDrawer.I.DrawWireSphere(clip.position, clip.radius, new Color(153 / 255f, 214 / 255f, 83 / 255f));
        }
    }
}