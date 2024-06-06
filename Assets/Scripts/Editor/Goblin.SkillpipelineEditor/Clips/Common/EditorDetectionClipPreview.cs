using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public abstract class EditorDetectionClipPreview<T> : PreviewBase<T> where T : ActionClip
    {
        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;
            if (time > clip.GetLength()) return;
            if (0 == time && previousTime > 0) return;
            OnUpdate(time, previousTime);
        }

        public abstract void OnUpdate(float time, float previousTime);
    }
}
