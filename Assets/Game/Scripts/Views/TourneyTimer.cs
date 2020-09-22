using UnityEngine;
using UnityEngine.UI;

public class TourneyTimer : MonoBehaviour
{
    public Text UserName;
    public Text Time;
    public Slider TimeBar;
    public Animator TimeBarColor;

    public float blinkOnSeconds = 30f;

    private bool isActive;

    public void SetUser(string userName, float maxUserTime)
    {
        UserName.text = userName;
        TimeBar.maxValue = maxUserTime;
    }

    public void SetSeconds(float seconds)
    {
        if (seconds <= blinkOnSeconds && isActive)
            TimeBarColor.SetBool("Blink", true);
        TimeBar.value = seconds;
        Time.text = Utils.SecondsToTimeFormat((int)Mathf.Ceil(seconds));
    }

    public void Switch(bool isActive)
    {
        if (this.isActive != isActive)
        {
            this.isActive = isActive;
            if (isActive && TimeBar.value != 0 && TimeBar.value <= blinkOnSeconds)
                TimeBarColor.SetBool("Blink", true);
            else
                TimeBarColor.SetTrigger(isActive ? "Red" : "Normal");
        }
    }
}
