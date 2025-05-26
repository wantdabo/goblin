using UnityEngine;
using UnityEngine.Playables;

namespace PipelineEditor
{
    public class PipelineWorkspace
    {
        /// <summary>
        /// 获取管线工作空间的 PlayableAsset
        /// </summary>
        /// <exception cref="Exception">未能正确寻找到对应资源</exception>
        public static PlayableAsset asset
        {
            get
            {
                var dire =  GameObject.Find("Pipeline").GetComponent<PlayableDirector>();
                if (null == dire)
                {
                    throw new System.Exception("Pipeline not found");
                }

                if (null == dire.playableAsset)
                {
                    throw new System.Exception("Pipeline playable asset not found");
                }

                return dire.playableAsset;
            }
        }
    }
}