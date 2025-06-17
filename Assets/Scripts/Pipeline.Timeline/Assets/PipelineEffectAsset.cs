using System.ComponentModel;
using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Common.Extensions;
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
                        
                        effgo.transform.position = data.position.ToVector3();
                        effgo.transform.eulerAngles = data.euler.ToVector3();
                        effgo.transform.localScale = Vector3.one * data.scale * Config.Int2Float;
                        
                        var followpos = Vector3.zero;
                        var followeuler = Vector3.zero;
                        var followscale = 1f;
                        switch (data.follow)
                        {
                            case EFFECT_DEFINE.FOLLOW_ACTOR:
                                if (null != PipelineWorkSpace.worker.modelgo)
                                {
                                    followpos = PipelineWorkSpace.worker.modelgo.transform.position;
                                    followeuler = PipelineWorkSpace.worker.modelgo.transform.eulerAngles;
                                    followscale = PipelineWorkSpace.worker.modelgo.transform.localScale.x;
                                }
                                break;
                            case EFFECT_DEFINE.FOLLOW_MOUNT:
                                // TODO 获取挂载点位置
                                break;
                        }

                        if (EFFECT_DEFINE.FOLLOW_NONE == data.followmask) return;
                        if (EFFECT_DEFINE.FOLLOW_POSITION == (data.followmask & EFFECT_DEFINE.FOLLOW_POSITION))
                        {
                            effgo.transform.position += followpos;
                        }
                        if (EFFECT_DEFINE.FOLLOW_ROTATION == (data.followmask & EFFECT_DEFINE.FOLLOW_ROTATION))
                        {
                            effgo.transform.eulerAngles += followeuler;
                        }
                        if (EFFECT_DEFINE.FOLLOW_SCALE == (data.followmask & EFFECT_DEFINE.FOLLOW_SCALE))
                        {
                            effgo.transform.localScale *= followscale;
                        }
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