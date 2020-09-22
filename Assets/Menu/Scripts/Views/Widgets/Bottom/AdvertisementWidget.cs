using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class AdvertisementWidget : Widget
{
    List<AdData> adsList;
    int currentAdIndex = 0;

    public Button button;
    public Image image;
    public Text text;
    private IEnumerator adsRoutine;

    public override void EnableWidget()
    {
        base.EnableWidget();
        SetAds(ContentController.AdsList);
    }

    private void SetAds(List<AdData> ads)
    {
        adsList = ads;
        if (adsList == null)
        {
            if(gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        StartRunningAds();
    }

    private void StartRunningAds()
    {
        if (adsRoutine != null)
            StopCoroutine(adsRoutine);
        if (adsList.Count > 1)
            adsRoutine = runAds();
        else
            adsRoutine = adsList[0].ImageData.LoadImageIEnumerator(null, s => ShowAd(adsList[0]));
        StartCoroutine(adsRoutine);
    }

    private void StopRunningAds()
    {
        if (adsRoutine != null)
            StopCoroutine(adsRoutine);
    }

    private IEnumerator runAds()
    {
        while (true)
        {
            if (currentAdIndex >= adsList.Count || currentAdIndex < 0)
                currentAdIndex = 0;
            yield return adsList[currentAdIndex].ImageData.LoadImageIEnumerator(null, s => ShowAd(adsList[currentAdIndex]));
            yield return new WaitForSeconds(adsList[currentAdIndex].ShowTime);
            currentAdIndex++;
        }
    }

    private void ShowAd(AdData ad)
    {
        if (button.gameObject.activeSelf == false)
            button.gameObject.SetActive(true);

        text.text = Utils.LocalizeTerm(ad.Text);
        image.sprite = ad.ImageData.Sprite;
        image.color = Color.white;
        button.onClick.RemoveAllListeners();
        if (ad.Callback != null)
            button.onClick.AddListener(ad.Callback);
#if UNITY_WEBGL
        if(ad.OpenWebPage)
            button.RegisterCallbackOnPressedDown();
        else
            button.UnregisterCallbackOnPressedDown();
#endif
    }
}
