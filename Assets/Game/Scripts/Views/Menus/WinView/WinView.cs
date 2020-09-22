using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GT.Websocket;

public class WinView : AbstractWinView
{
    #region Public Members
    public GameObject PlayerTextBubble;
    public GameObject OpponentTextBubble;

    public Image PlayerTimeBarFill;
    public Image OpponentTimeBarFill;
    
    public Text WinAmountText;
    public Text PlayerBalanceText;
    public Text DoubleAmount;
    public Text DoubleFeeText;

    public Text PlayerPointsWonText;

    public Button Double;
    public Button PlayVsNewOpponent;
    public Button AddFriend;
    public Button Share;

    #endregion Public Members

    #region Private Members
    private float waitingTime = 20f;
    private bool opponentAnswered = false;
    private bool playerAnswered = false;
    #endregion Private Members

    #region Public Functions

    private void Start()
    {
        WebSocketKit.Instance.ServiceEvents[ServiceId.AcceptDouble] += WebSocketKit_RecieveDouble;
        WebSocketKit.Instance.ServiceEvents[ServiceId.RefuseDouble] += WebSocketKit_RecieveDouble;
    }

    private void OnDestroy()
    {
        WebSocketKit.Instance.ServiceEvents[ServiceId.AcceptDouble] -= WebSocketKit_RecieveDouble;
        WebSocketKit.Instance.ServiceEvents[ServiceId.RefuseDouble] -= WebSocketKit_RecieveDouble;
    }

    private void WebSocketKit_RecieveDouble(Service service)
    {
        RematchService rematch = service as RematchService;
        if (rematch.senderId == UserController.Instance.gtUser.Id)
            PlayerAnswerRematch(rematch.isRematch);
        else
            OpponentAnswerRematch(rematch.isRematch);
    }

    public void ShowWinAmount(Enums.MatchKind kind, float winAmount, float fee, float gameLoyalty)
    {
        string PostFix = Wallet.MatchKindToPrefix(kind);

        if (winAmount > 0)
            TrackingKit.SetVictory(winAmount);
        else
            TrackingKit.VictoryStreak = 0;

        float AvailableMoneyAmount = UserController.Instance.wallet.AvailableCurrency(kind);
        string AvailableMoney = Wallet.MatchKindToPrefix(kind);

        switch (kind)
        {
            case Enums.MatchKind.Cash:
                AvailableMoney += Wallet.AmountToString(AvailableMoneyAmount, 2);
                break;
            case Enums.MatchKind.Virtual:
                AvailableMoney += ((int)AvailableMoneyAmount).ToString();
                break;
            default:
                AvailableMoney += AvailableMoneyAmount.ToString("#,##0");
                break;
        }

        WinAmountText.text = PostFix + winAmount.ToString();

        PlayerPointsWonText.text = string.Format("+ {0}", Wallet.LoyaltyPointsPostfix + gameLoyalty.ToString("#,##0"));

        string Loyalty = Wallet.LoyaltyPointsPostfix + UserController.Instance.wallet.LoyaltyPoints.ToString("#,##0");
        PlayerBalanceText.text = Utils.LocalizeTerm("Balance") + string.Format(": {0} | <color=#DE3C3EFF>{1}</color>", AvailableMoney, Loyalty);

        ActivateDoubleButton(AvailableMoneyAmount >= winAmount * 2);

        DoubleAmount.text = Utils.LocalizeTerm("Raise to {0}", PostFix + winAmount.ToString());
        DoubleFeeText.text = Utils.LocalizeTerm("Fee") + " " + PostFix + (fee * 2).ToString();

        StartCoroutine(StartPlayerTimer());
        StartCoroutine(StartOpponentTimer());
    }

    public void DoubleButton()
    {
        PlayerAnswerRematch(true);
        RemoteGameController.Instance.Rematch();
    }

    public void ActivateDoubleButton(bool active)
    {
        Double.interactable = active;
    }

    public void AddFiendButton()
    {
        RemoteGameController.Instance.AddFriend();
    }

    private void PlayerAnswerRematch(bool isRematch)
    {
        playerAnswered = true;
        ActivateDoubleButton(false);
        PlayerTextBubble.SetActive(true);
        PlayerTextBubble.GetComponentInChildren<Text>().text = Utils.LocalizeTerm(isRematch ? "Let's Play!" : "Maybe Next Time");
    }

    private void OpponentAnswerRematch(bool isRematch)
    {
        OpponentTextBubble.SetActive(true);
        OpponentTextBubble.GetComponentInChildren<Text>().text = Utils.LocalizeTerm(isRematch ? "Let's Play!" : "Maybe Next Time");
        opponentAnswered = true;

        if(!isRematch)
            ActivateDoubleButton(false);
    }

    public IEnumerator StartOpponentTimer()
    {
        OpponentTimeBarFill.gameObject.SetActive(true);
        opponentAnswered = false;

        float counter = waitingTime;
        while (counter > 0 && !opponentAnswered)
        {
            OpponentTimeBarFill.fillAmount = counter / waitingTime;
            yield return null;
            counter -= Time.deltaTime;
        }

        if (!opponentAnswered)
            OpponentAnswerRematch(false);

        OpponentTimeBarFill.gameObject.SetActive(false);
    }

    public IEnumerator StartPlayerTimer()
    {
        PlayerTimeBarFill.gameObject.SetActive(true);
        playerAnswered = false;
        float counter = waitingTime;
        while (counter > 0 && !playerAnswered)
        {
            PlayerTimeBarFill.fillAmount = counter / waitingTime;
            yield return null;
            counter -= Time.deltaTime;
        }

        if (!playerAnswered)
        {
            PlayerAnswerRematch(false);
            RequestInGameController.Instance.SendRematch(false);
        }

        PlayerTimeBarFill.gameObject.SetActive(false);
    }

    public override void Reset()
    {
        OpponentTextBubble.SetActive(false);
        PlayerTextBubble.SetActive(false);
        ActivateDoubleButton(true);
        PlayerTimeBarFill.fillAmount = 1f;
        OpponentTimeBarFill.fillAmount = 1f;
        PlayerTimeBarFill.gameObject.SetActive(false);
        OpponentTimeBarFill.gameObject.SetActive(false);
        base.Reset();
    }

    #endregion Public Functions
}
