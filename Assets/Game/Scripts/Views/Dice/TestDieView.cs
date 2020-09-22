using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GT.Backgammon.View
{
    public class TestDieView : MonoBehaviour
    {
        public Text dieText;
        public CanvasGroup canvasGroup;
        public Image background;

        public int Die;
        public bool Used;

        public void ShowDie(int die, Color bgColor, Color textColor)
        {
            gameObject.SetActive(true);
            Die = die;
            background.color = bgColor;
            canvasGroup.alpha = 1;
            Used = false;
            dieText.color = textColor;
            dieText.text = die.ToString();
        }

        public void HideDie()
        {
            Die = 0;
            dieText.text = string.Empty;
            gameObject.SetActive(false);
        }
        
        public void DieUsed(bool used)
        {
            Used = used;
            canvasGroup.alpha = used ? .5f : 1f;
        }
    }
}
