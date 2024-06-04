using System.Collections.Generic;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public interface IDirector : IData
    {
        float Length { get; }
        // void Validate();

        void SaveToAssets();
    }
}