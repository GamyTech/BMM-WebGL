using UnityEngine;
using UnityEngine.UI;

public class SpectatorOverlayView : MonoBehaviour
{
    public delegate void NextMatch();
    public event NextMatch OnNextMatch = delegate { };

    public FadingElement TitleMessage;
    public Text TitleText;
    public Slider TimeProgress;

    private bool isTimerRunning = false;

    public void ShowMessage(string text)
    {
        TitleMessage.FadeIn();
        TitleText.text = text;
    }

    public void ShowLoading()
    {
        ShowMessage("Loading...");
    }

    public void HideTitle()
    {
        TitleMessage.FadeOut();
    }

    public void SetMaxTimeValue(float maxTime)
    {
        TimeProgress.maxValue = maxTime;
    }

    public void StartTimer()
    {
        isTimerRunning = true;
        TimeProgress.value = 0;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        TimeProgress.value = TimeProgress.maxValue;
    }

    private void Update()
    {
        if (isTimerRunning && TimeProgress.value < TimeProgress.maxValue)
        {
            TimeProgress.value += Time.deltaTime;
            if (TimeProgress.value >= TimeProgress.maxValue)
            {
                StopTimer();
                OnNextMatch();
            }
        }
    }

    public void PauseTime(bool isOn)
    {
        isTimerRunning = !isOn;
    }
}
