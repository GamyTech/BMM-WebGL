using UnityEngine;
using UnityEngine.UI;
using System;
using GT.Websocket;
using GT.User;

public class EarnMoreBonusWidget : Widget {

    public Image image;
    public Button button;
    public Text text;
    public ParticalesDirect particlesDirect;
    public SmallLoadingView smallLoadingView;

    float velocity = 0f;


    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.CollectTimelyBonus] += OnCollectTimelyBonus;
        UserController.Instance.gtUser.OnTimelyBonusChanged += GtUser_OnTimelyBonusChanged;
        HandleTimelyBonus(UserController.Instance.gtUser.TimelyBonusData);
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.CollectTimelyBonus] -= OnCollectTimelyBonus;
        UserController.Instance.gtUser.OnTimelyBonusChanged -= GtUser_OnTimelyBonusChanged;

        base.DisableWidget();
    }

    void Update()
    {
        if (button.interactable || UserController.Instance.gtUser.TimelyBonusData == null)
            return;

        float progress = Math.Min(1f, UserController.Instance.gtUser.TimelyBonusData.Progress);

        if (image.fillAmount != progress)
            image.fillAmount = Mathf.SmoothDamp(image.fillAmount, progress, ref velocity, 0.4f);

        if (progress < 1f)
        {
            TimeSpan timeLeft = UserController.Instance.gtUser.TimelyBonusData.NextUnlockTime.Subtract(DateTime.UtcNow);
            text.text = Utils.LocalizeTerm("Next Gift") + " " + 
                timeLeft.Hours.ToString("00") + ":" + 
                timeLeft.Minutes.ToString("00") + ":" + 
                timeLeft.Seconds.ToString("00");
        }
        else
            button.interactable = true;
    }

    private void HandleTimelyBonus(TimelyBonus bonus)
    {
        if (bonus != null)
        {
            bool ready = bonus.IsReady;
            button.interactable = ready;
            if (ready)
            {
                text.text = Utils.LocalizeTerm("Collect Your Gift Now");
                image.fillAmount = 1f;
            }
        }
    }

    public void CollectPrice()
    {
        if(!button.interactable)
        {
            Debug.LogError("try to collect when the button is not intercative");
            return;
        }

        button.interactable = false;
        smallLoadingView.ShowLoading(transform as RectTransform);
        UserController.Instance.SendCollectTimelyBonus();
    }

    public void ScrollToPosition()
    {
        SmoothScroll smoothScroll = GetComponentInParent<SmoothScroll>();

        if (smoothScroll!= null)
            smoothScroll.ScrollToMax(transform as RectTransform);
    }

    #region Callbacks and Events
    private void GtUser_OnTimelyBonusChanged(TimelyBonus newValue)
    {
        smallLoadingView.HideLoading();
        HandleTimelyBonus(newValue);
    }

    private void OnCollectTimelyBonus(Ack ack)
    {
        smallLoadingView.HideLoading();
        switch (ack.Code)
        {
            case WSResponseCode.OK:
                smallLoadingView.HideLoading();

                GameObject particlesTarget = GameObject.FindGameObjectWithTag("ParticlesTarget");
                if(particlesTarget == null)
                {
                    Debug.LogError("Failed to find ParticlesTarget");
                    particlesTarget = gameObject;
                }

                particlesDirect.Move(particlesTarget.transform as RectTransform);
                MenuSoundController.Instance.Play(Enums.MenuSound.CollectBonus);
                break;
            default:
                PopupController.Instance.ShowSmallPopup(Utils.LocalizeTerm("Unexpected error, Try again or contact support. code: {0}", (int)ack.Code),
                    new SmallPopupButton("Try Again", CollectPrice), new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)));
                break;
        }
    }
    #endregion Callbacks
}
