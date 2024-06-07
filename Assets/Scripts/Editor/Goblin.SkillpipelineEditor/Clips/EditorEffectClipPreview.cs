using Goblin.SkillPipelineEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 音频预览
    /// </summary>
    [CustomPreview(typeof(EditorEffectClip))]
    public class EditorEffectClipPreview : PreviewBase<EditorEffectClip>
    {
        private GameObject effect;
        private List<ParticleSystem> pss;

        public override void Enter()
        {
            if (effect == null)
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(clip.resPath);
                if (null == obj) return;
                effect = GameObject.Instantiate(obj);
                effect.transform.position = Vector3.zero;
            }

            ResolveEffect(effect);
        }

        private void ResolveEffect(GameObject effectObj)
        {
            if (null == effectObj) return;
            pss = effect.GetComponentsInChildren<ParticleSystem>().ToList();
            if (null == pss) return;
            foreach (var ps in pss)
            {
                if (false == ps.isPlaying && false == ps.useAutoRandomSeed) ps.useAutoRandomSeed = false;
                ps.Play();
            }
        }

        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;

            if (null == effect) return;
            effect.transform.position = clip.position;
            effect.transform.rotation = Quaternion.Euler(clip.euler);
            effect.transform.localScale = clip.scale * Vector3.one;
            effect.SetActive(time < clip.GetLength());

            if (null == pss) return;
            foreach (var ps in pss)
            {
                ps.Simulate(time);
            }
        }
    }
}