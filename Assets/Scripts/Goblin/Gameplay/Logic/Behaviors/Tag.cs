using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class Tag : Behavior<TagInfo>
    {
        public bool Has(ushort key)
        {
            return info.tags.ContainsKey(key);
        }
        
        public void Rmv(ushort key)
        {
            if (info.tags.ContainsKey(key)) info.tags.Remove(key);
        }
        
        public bool Get(ushort key, out int tag)
        {
            return info.tags.TryGetValue(key, out tag);
        }
        
        public void Set(ushort key, int tag)
        {
            if (info.tags.ContainsKey(key)) info.tags.Remove(key);
            
            info.tags.Add(key, tag);
        }
    }
}