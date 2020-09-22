using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GT.Backgammon.Player;
using System;
using GT.Backgammon.Logic;

public class DoubleTheBetView : MonoBehaviour
{
    #region Public Members
    public FadingElement fadingBackground;

    public Image PlayerImage;
    public Image OpponentImage;
    public Image loadingFill;
    public Image PlayerLuckyItem;
    public Image OpponentLuckyItem;

    public Text DoubleAmountText;
    public Text DoubleAmountFeeText;
    public Text DoubleTwiceAmountText;
    public Text DoubleTwiceAmountFeeText;
    public Text BalanceText;

    public GameObject DoubleAgainButtonGO;
    public GameObject HideButtonGO;
    public GameObject Content;

    Action YesAction;
    Action NoAction;
    Action DoubleAgainAction;
    #endregion Public Members
    
    private float waitingTime = 30f;

    #region Public Function

    public void Init(IPlayer[] players, DoubleRequestEventArgs args, Action yes, Action no, Action doubleAgain)
    {
        gameObject.SetActive(true);

        IPlayer localPlayer;
        IPlayer remotePlayer;
        if (players[0].playerId.Equals(UserController.Instance.gtUser.Id))
        {
            localPlayer = players[0];
            remotePlayer = players[1];
        }
        else
        {
            localPlayer = players[1];
            remotePlayer = players[0];
        }

        SetImage(localPlayer.playerData, PlayerImage);
        SetLuckyitemImage(localPlayer.playerData, PlayerLuckyItem);

        SetImage(remotePlayer.playerData, OpponentImage);
        SetLuckyitemImage(remotePlayer.playerData, OpponentLuckyItem);

        if (TourneyController.Instance.IsInTourney())
        {
            DoubleAmountText.transform.parent.gameObject.SetActive(false);
            DoubleTwiceAmountText.transform.parent.gameObject.SetActive(false);
            BalanceText.text = "";
        }
        else
        {
            DoubleAmountText.text = args.NewBet;
            DoubleAmountFeeText.text = Utils.LocalizeTerm("Fee") + " " + args.NewFee;
            DoubleTwiceAmountText.text = args.DoubleAgainBet;
            DoubleTwiceAmountFeeText.text = Utils.LocalizeTerm("Fee") + " " + args.DoubleAgainFee;
            BalanceText.text = Utils.LocalizeTerm("Balance").ToUpper() + " " + Wallet.MatchKindToPrefix(args.Kind) + UserController.Instance.wallet.AvailableCurrency(AppInformation.MATCH_KIND);
        }

        YesAction = yes;
        NoAction = no;
        if(doubleAgain == null)
            DoubleAgainButtonGO.SetActive(false);
        else
        {
            DoubleAgainButtonGO.SetActive(true);
            DoubleAgainAction = doubleAgain;
        }

        HideButtonGO.SetActive(true);
        fadingBackground.FadeIn();

        StartCoroutine(StartWaitForPlayer());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void YesButton()
    {
        Debug.Log("YesButton");
        YesAction();
    }

    public void DoubleAgainButton()
    {
        Debug.Log("DoubleAgainButton");
        DoubleAgainAction();
    }

    public void NoButton()
    {
        Debug.Log("NoButton");
        NoAction();
    }

    public void HideView()
    {
        StartCoroutine(On());
    }

    public void ShowView()
    {
        StartCoroutine(Off());
    }

    #endregion Public Function

    public IEnumerator On()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewShow);
        float start = 1f;
        float end = 0.05f;

        while (!Aprox(start, end))
        {
            yield return null;
            start = Change.Lerp(start, end, 0.2f);
            fadingBackground.canvasGroup.alpha = start;
        }
    }

    public IEnumerator Off()
    {
        GameSoundController.Instance.PlayNonSpecificEffect(Enums.GameSound.ViewHide);
        float end = 1f;
        float start = 0.05f;

        while (!Aprox(start,end))
        {
            yield return null;
            start = Change.Lerp(start, end, 0.2f);
            fadingBackground.canvasGroup.alpha = start;
        }
        fadingBackground.canvasGroup.alpha = 1;

    }

    public IEnumerator StartWaitForPlayer()
    {
        float waitTo = waitingTime;
        bool vibrated = false;

        while (waitTo > 0)
        {
            yield return null;
            waitTo -= Time.deltaTime;
            loadingFill.fillAmount = waitTo / waitingTime;

            if(!vibrated && loadingFill.fillAmount <= 0.33f)
            {
                vibrated = true;
                SettingsController.Instance.Vibrate();
            }
        }

        NoAction();
    }

    #region Private Function
    private void SetImage(PlayerData data, Image image)
    {
        if (data == null || data.Avatar == null)
            return;

        data.Avatar.LoadImage(this, s => { image.sprite = data.Avatar.Sprite; }, AssetController.Instance.DefaultAvatar);
    }

    private void SetLuckyitemImage(PlayerData Data, Image image)
    {
        string[] luckyItems;
        if (Data.SelectedItems.TryGetValue(Enums.StoreType.LuckyItems, out luckyItems) && luckyItems != null && luckyItems.Length > 0)
            UserController.Instance.gtUser.StoresData.GetItem(Enums.StoreType.LuckyItems, luckyItems[0]).LocalSpriteData.LoadImage(this, s => { image.sprite = s; });
    }

    private bool Aprox(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.05f;
    }

    #endregion Private Function
}
