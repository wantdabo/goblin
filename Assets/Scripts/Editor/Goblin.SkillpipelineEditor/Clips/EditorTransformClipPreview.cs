using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 变化预览
    /// </summary>
    [CustomPreview(typeof(EditorTransformClip))]
    public class EditorTransformClipPreview : PreviewBase<EditorTransformClip>
    {
        public override void Reverse()
        {
            base.Reverse();
            App.AssetData.cloneModel.transform.position -= clip.position;
            var euler = App.AssetData.cloneModel.transform.rotation.eulerAngles - clip.eulerAngle;
            App.AssetData.cloneModel.transform.rotation = Quaternion.Euler(euler);
            App.AssetData.cloneModel.transform.localScale -= Vector3.one * clip.scale;
        }

        public override void Enter()
        {
            base.Enter();
            App.AssetData.cloneModel.transform.position += clip.position;
            var euler = App.AssetData.cloneModel.transform.rotation.eulerAngles + clip.eulerAngle;
            App.AssetData.cloneModel.transform.rotation = Quaternion.Euler(euler);
            App.AssetData.cloneModel.transform.localScale += Vector3.one * clip.scale;
        }

        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;

            Debug.Log(App.AssetData.cloneModel);
        }
    }
}
