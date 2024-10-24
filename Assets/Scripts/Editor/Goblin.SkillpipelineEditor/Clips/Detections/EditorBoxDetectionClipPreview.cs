using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 立方体碰撞预览
    /// </summary>
    [CustomPreview(typeof(EditorBoxDetectionClip))]
    public class EditorBoxDetectionClipPreview : EditorDetectionClipPreview<EditorBoxDetectionClip>
    {
        public override void OnUpdate(float time, float previousTime)
        {
            GizmosDrawer.I.DrawWireCube(App.AssetData.cloneModel.transform.position + clip.position, clip.size, new Color(153 / 255f, 214 / 255f, 83 / 255f));
        }
    }
}
