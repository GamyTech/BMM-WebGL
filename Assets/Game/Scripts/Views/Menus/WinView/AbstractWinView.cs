using UnityEngine;
using UnityEngine.UI;
using GT.Backgammon.Player;

public class AbstractWinView : FadingElement
{
    #region Public Members
    public Text WinnerText;

    public Text PlayerNameText;
    public Text PlayerDoublesText;
    public Text PlayerHitCheckersText;
    public Text PlayerMoveCountText;

    public Text OpponentNameText;
    public Text OpponentDoublesText;
    public Text OpponentHitCheckersText;
    public Text OpponentMoveCountText;

    public Image PlayerImage;
    public Image OpponentImage;

    public Image PlayerLuckyItem;
    public Image OpponentLuckyItem;

    public Button Home;
    #endregion Public Members

    #region Protected Members
    protected PlayerData p1Data;
    protected PlayerData p2Data;
    #endregion Protected Members

    #region Public Functions
    public void InitPlayersData(IPlayer userPlayer, IPlayer opponentPlayer)
    {
        p1Data = userPlayer.playerData;
        p2Data = opponentPlayer.playerData;

        PlayerNameText.text = userPlayer.playerData.UserName;
        OpponentNameText.text = opponentPlayer.playerData.UserName;
    }

    private void SetImage(PlayerData Data, Image image)
    {
        if (Data == null || Data.Avatar == null)
            return;

        Data.Avatar.LoadImage(this, s => { image.sprite = s; }, AssetController.Instance.DefaultAvatar);
    }

    private void SetLuckyitemImage(PlayerData Data, Image image)
    {
        string[] luckyItems;
        if (Data.SelectedItems.TryGetValue(Enums.StoreType.LuckyItems, out luckyItems) && luckyItems != null && luckyItems.Length > 0)
            UserController.Instance.gtUser.StoresData.GetItem(Enums.StoreType.LuckyItems, luckyItems[0]).LocalSpriteData.LoadImage(this, s => { image.sprite = s; });
    }

    public void Show(PlayerStats localPlayer, PlayerStats remotePlayer, IPlayer winner)
    {
        SetActive(true);

        if (localPlayer == null || remotePlayer == null)
        {
            Debug.LogError("null stats info");
            return;
        }

        if (winner.playerId == UserController.Instance.gtUser.Id)
            GameSoundController.Instance.PlayVictorySound();

        SetImage(p1Data, PlayerImage);
        SetLuckyitemImage(p1Data, PlayerLuckyItem);
        SetImage(p2Data, OpponentImage);
        SetLuckyitemImage(p2Data, OpponentLuckyItem);

        PlayerDoublesText.text = Utils.LocalizeTerm("Doubles: {0}", localPlayer.doubles);
        PlayerHitCheckersText.text = Utils.LocalizeTerm("Hit Checkers: {0}", localPlayer.eaten);
        PlayerMoveCountText.text = Utils.LocalizeTerm("Six Five: {0}", localPlayer.sixFive);

        OpponentDoublesText.text = Utils.LocalizeTerm("Doubles: {0}", remotePlayer.doubles);
        OpponentHitCheckersText.text = Utils.LocalizeTerm("Hit Checkers: {0}", remotePlayer.eaten);
        OpponentMoveCountText.text = Utils.LocalizeTerm("Six Five: {0}", remotePlayer.sixFive);

        string Winnername = winner.playerData.UserName.Length > 10 ? winner.playerData.UserName.Substring(0, 8) + "..." : winner.playerData.UserName;
        WinnerText.text = Utils.LocalizeTerm("{0} " + Utils.LocalizeTerm("WON"), Winnername.ToUpper());
    }

    public void SetActive(bool active)
    {
        if (active)
            base.FadeIn();
        else
            base.FadeOut();
    }

    public void HomeButton()
    {
        RemoteGameController.Instance.ReturnHome();
    }

    public virtual void Reset()
    {
        SetActive(false);
    }

    #endregion Public Functions
}
