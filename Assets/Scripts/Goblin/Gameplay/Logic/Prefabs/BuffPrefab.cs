using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Prefabs.Common;

namespace Goblin.Gameplay.Logic.Prefabs
{
    public struct BuffPrefabInfo : IPrefabInfo
    {
    }

    public class BuffPrefab : Prefab<BuffPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.BUFF;
        
        protected override void OnProcessing(ulong actor, BuffPrefabInfo info)
        {
        }
    }
}