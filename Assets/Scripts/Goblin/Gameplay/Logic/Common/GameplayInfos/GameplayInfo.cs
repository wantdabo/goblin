using MessagePack;

namespace Goblin.Gameplay.Logic.Common.GameplayInfos
{
    public class GameplayInfo
    {
        public int seed { get; set; }
        public PlayerInfo[] players { get; set; }
    }
}