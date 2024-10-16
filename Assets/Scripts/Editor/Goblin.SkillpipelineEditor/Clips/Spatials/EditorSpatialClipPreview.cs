﻿using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 变化预览
    /// </summary>
    [CustomPreview(typeof(EditorSpatialClip))]
    public class EditorSpatialClipPreview : PreviewBase<EditorSpatialClip>
    {
        public override void Reverse()
        {
            base.Reverse();
            App.AssetData.cloneModel.transform.position -= clip.position;
            App.AssetData.cloneModel.transform.localScale -= Vector3.one * clip.scale;
        }

        public override void Enter()
        {
            base.Enter();
            App.AssetData.cloneModel.transform.position += clip.position;
            App.AssetData.cloneModel.transform.localScale += Vector3.one * clip.scale;
        }

        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;

            Debug.Log(App.AssetData.cloneModel);
        }
    }
}
