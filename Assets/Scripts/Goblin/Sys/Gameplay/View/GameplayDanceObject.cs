using UnityEngine;
using UnityEngine.UI;
using uTools;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayDanceObject
    {
        public GameObject go { get; private set; }
        public TweenPosition tweenposition { get; private set; }
        public TweenScale tweenscale { get; private set; }
        public Text desc { get; private set; }

        public GameplayDanceObject(GameObject go)
        {
            this.go = go;
            tweenposition = go.GetComponent<TweenPosition>();
            tweenscale = go.GetComponent<TweenScale>();
            desc = go.transform.Find("Desc").GetComponent<Text>();
        }

        public void Settings(uint value)
        {
            var x = Random.Range(-50f, 50f);
            tweenposition.from = go.transform.localPosition + new Vector3(x, Random.Range(25, 50), 0);
            tweenposition.to = go.transform.localPosition + new Vector3(x, Random.Range(50, 150), 0);
            desc.text = value.ToString();
        }

        public void Play()
        {
            tweenposition.enabled = true;
            tweenposition.ResetToBeginning();
            tweenposition.PlayForward();
            tweenposition.enabled = true;
            tweenscale.ResetToBeginning();
            tweenscale.PlayForward();
        }
    }
}
