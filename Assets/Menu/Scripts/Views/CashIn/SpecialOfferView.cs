using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SpecialOfferView : MonoBehaviour
{
    public Image offerImage;
    public Text topText;
    public Text midText;
    public Text bottomText;
    public Text startOfferExpirationText;
    public Text endOfferExpirationText;
    public Text expirationTimeText;
    public GameObject ExpirationPanel;
    public SmoothResize resizer;
    public Sprite defaultBackground;

    private DateTime expirationDate;


    private bool expired = false;

    void Update()
    {
        if (expired == false)
            RefreshTimeTexts(expirationDate);
    }

    public void PopulateSpecialOffer(SpecialDepositOffer offer)
    {
        topText.text = offer.Title;
        midText.text = offer.Name;
        bottomText.text = offer.Description;
        expirationDate = offer.Expiration;
        HandleDate(expirationDate);
        HandleImage(offer.ImageData);
    }

    #region Aid Functions
    private void HandleImage(SpriteData spriteData)
    {
        spriteData.LoadImage(this, s => { offerImage.sprite = s; }, defaultBackground);
    }

    private void HandleDate(DateTime date)
    {
        expired = (date - DateTime.Now).TotalSeconds <= 0;
        ExpirationPanel.SetActive(!expired);
    }

    private string createExpirationText(TimeSpan timeLeft)
    {
        int days = Mathf.Abs(timeLeft.Days);
        return (days > 0 ? days.ToString("00") + ":" : "") +
            string.Format("{0}:{1}:{2}",
                Mathf.Abs(timeLeft.Hours).ToString("00"),
                Mathf.Abs(timeLeft.Minutes).ToString("00"),
                Mathf.Abs(timeLeft.Seconds).ToString("00"));
    }

    private void RefreshTimeTexts(DateTime expiration)
    {
        TimeSpan timeLeft = expiration - DateTime.Now;
        startOfferExpirationText.text = timeLeft.TotalSeconds > 0 ? Utils.LocalizeTerm("Offer ends in") : Utils.LocalizeTerm("Offer ended");
        expirationTimeText.text = createExpirationText(timeLeft);
    }
    #endregion Aid Functions
}
