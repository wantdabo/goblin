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
        private List<Animator> animators;

        public override void Enter()
        {
            if (effect == null)
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(clip.res);
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
            animators = effect.GetComponentsInChildren<Animator>().ToList();
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
            var posoffset = Vector3.zero;
            if (clip.binding) posoffset = App.AssetData.cloneModel.transform.position;
            effect.transform.position = posoffset + clip.position;
            effect.transform.rotation = Quaternion.Euler(clip.eulerAngle);
            effect.transform.localScale = clip.scale * Vector3.one;
            effect.SetActive(time < clip.GetLength());

            if (null != pss)
            {
                foreach (var ps in pss)
                {
                    ps.Simulate(time);
                }
            }
            
            if (null != animators)
            {
                foreach (var animator in animators)
                {
                    animator.Update(time);
                }
            }
        }
    }
}