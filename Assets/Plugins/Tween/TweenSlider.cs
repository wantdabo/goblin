using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTools
{

    public class TweenSlider : Tween<float>
    {

        private float mValue;

        private Slider mSlider;
        Slider cacheSlider
        {
            get
            {
                mSlider = GetComponent<Slider>();
                if (mSlider == null)
                {
                    Debug.LogError("'uTweenSlider' can't find 'Slider'");
                }
                return mSlider;
            }
        }

        public float sliderValue
        {
            set
            {
                cacheSlider.value = Mathf.Clamp01(value);
            }
        }

        public override float value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                sliderValue = value;
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }

        public static TweenSlider Begin(Slider slider, float from, float to, float duration, float delay)
        {
            TweenSlider comp = Begin<TweenSlider>(slider.gameObject, duration);
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.delay = delay;

            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
