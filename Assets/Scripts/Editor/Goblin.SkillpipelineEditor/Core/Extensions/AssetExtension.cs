using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public static class AssetExtension
    {
        public static float TimeToPos(this Asset asset, float time)
        {
            return (time - asset.ViewTimeMin) / asset.ViewTime * G.CenterRect.width;
        }

        public static float PosToTime(this Asset asset, float pos)
        {
            return (pos - Styles.LEFT_MARGIN) / G.CenterRect.width * asset.ViewTime + asset.ViewTimeMin;
        }
    }
}