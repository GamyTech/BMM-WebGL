using UnityEngine;
using UnityEngine.UI;

public class TourneyTimerView : MonoBehaviour
{
    public GameObject timerView;
    public Text tourneyTimeText;
    public Animator tourneyTimeAnimator;
    public float tourneyTimerBlinkSeconds;

    public TourneyEndWaitingView tourneyEndView;

    public int ShowTourneyTimerOn = 300;

    private bool isTicking;

    private void Start()
    {
        isTicking = !TourneyController.Instance.IsInTourney();
    }

    public void Update()
    {
        if (isTicking) return;

        float time = TourneyController.Instance.GetSecondsLeft();
        timerView.SetActive(time <= ShowTourneyTimerOn);
        if (timerView.activeSelf)
        {
            tourneyTimeText.text = Utils.SecondsToTimeFormat((int)time);
            if (time < tourneyTimerBlinkSeconds)
                tourneyTimeAnimator.SetBool("Blink", true);
            if (time <= 0)
            {
                tourneyEndView.ShowLoading();
                isTicking = true;
            }
        }
    }
}
