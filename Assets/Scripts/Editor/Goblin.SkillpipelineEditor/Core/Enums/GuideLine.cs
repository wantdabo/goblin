using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    internal struct GuideLine
    {
        public float time;
        public Color color;

        public GuideLine(float time, Color color)
        {
            this.time = time;
            this.color = color;
        }
    }
}