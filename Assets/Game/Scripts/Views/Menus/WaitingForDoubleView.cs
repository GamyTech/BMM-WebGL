using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaitingForDoubleView : SmallLoadingView
{
    public Text loadingText;
    public Image loadingFill;
    private bool opponentAnswered = false;

    public IEnumerator StartWaitForOpponent(float waitingTime)
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewShow);
        loadingText.text = "Asking for Double";
        ShowLoading(transform as RectTransform, null, false);

        opponentAnswered = false;
        float waitTo = waitingTime;
        while (waitTo > 0 && !opponentAnswered)
        {
            yield return null;
            waitTo -= Time.deltaTime;
            loadingFill.fillAmount = waitTo / waitingTime;
        }
    }


    public void StopWaitForOpponent()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewHide);
        opponentAnswered = true;
        base.HideLoading(null, true);
    }

}
