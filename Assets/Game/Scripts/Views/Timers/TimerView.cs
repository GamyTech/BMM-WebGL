using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GT.Backgammon.View
{
    public class TimerView : MonoBehaviour
    {
        public Slider innerSlider;
        public Slider outerSlider;

        readonly Color innerMinColor = Color.green;
        readonly Color innerMaxColor = Color.red;
        readonly Color outerMinColor = Color.cyan;
        readonly Color outerMaxColor = Color.black;

        public void SetTimer(float innerValue, float outerValue)
        {
            innerSlider.value = innerValue;
            innerSlider.fillRect.GetComponent<Image>().color = Color.Lerp(innerMaxColor, innerMinColor, innerValue);

            outerSlider.value = outerValue;
            outerSlider.fillRect.GetComponent<Image>().color = Color.Lerp(outerMaxColor, outerMinColor, outerValue);
        }
    }
}
