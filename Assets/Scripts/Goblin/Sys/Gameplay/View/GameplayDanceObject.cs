using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayDanceObject
    {
        public GameObject go { get; private set; }
        public Text desc { get; private set; }
        
        public GameplayDanceObject(GameObject go)
        {
            this.go = go;
            desc = go.GetComponent<Text>();
        }

        public void Settings(uint value)
        {
            desc.text = value.ToString();
        }
    }
}
