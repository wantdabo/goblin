using System.ComponentModel;
using Animancer;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Render.Common;
using Goblin.Misc;
using Pipeline.Timeline.Assets.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("特效指令")]
    public class PipelineEffectAsset : PipelineAsset<PipelineEffectAsset.PipelineEffectBehavior, EffectData>
    {
        public class PipelineEffectBehavior : PipelineBehavior<EffectData>
        {
            /// <summary>
            /// 特效资源路径
            /// </summary>
            private string res { get; set; }
            /// <summary>
            /// 特效 GameObject
            /// </summary>
            private GameObject effgo { get; set; }

            public override void OnGraphStop(Playable playable)
            {
                base.OnGraphStop(playable);
                if (null == effgo) return;
                res = null;
                DestroyImmediate(effgo);
            }

            protected override void OnExecute(Playable playable, FrameData info)
            {
                base.OnExecute(playable, info);
                if (false == EditorConfig.location.EffectInfos.TryGetValue(data.effect, out var effcfg))
                {
                    res = null;
                    DestroyImmediate(effgo);
                    return;
                }

                if (null != res && res.Equals(effcfg.Res))
                {
                    if (null != effgo)
                    {
                        var controller = effgo.GetComponent<EffectController>();
                        if (null == controller) return;
                        controller.Simulate((float)playable.GetTime());
                    }

                    return;
                }
                if (null != effgo) DestroyImmediate(effgo);
                
                res = effcfg.Res;
                effgo = EditorRes.LoadEffect(res);
            }
        }
    }
}