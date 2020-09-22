using UnityEngine;
using UnityEngine.UI;
using GT.Backgammon.Player;

public class ReplayMenuView : MonoBehaviour
{
    #region Public Members

    public GameLogsView gameLogsView;

    public Text CurrentBetText;
    public Text MaxBetText;

    public Text BottomPlayerNameText;
    public Text TopPlayerNameText;

    public Image BottomPlayerSprite;
    public Image TopPlayerSprite;
    #endregion Public Members

    #region Public Functions

    public void SetPlayersData(PlayerData bottomPlayer, PlayerData topPlayer)
    {
        if (bottomPlayer == null || topPlayer == null)
            return;

        BottomPlayerNameText.text = bottomPlayer.UserName;
        SetProfilePicture(bottomPlayer, BottomPlayerSprite, bottomPlayer.Color == PlayerColor.White);

        TopPlayerNameText.text = topPlayer.UserName;
        SetProfilePicture(topPlayer, TopPlayerSprite, topPlayer.Color == PlayerColor.White);
    }

    public void SetStakeData(string currentBet, string maxBet)
    {
        CurrentBetText.text = currentBet;
        MaxBetText.text = maxBet;
    }

    public void Reset()
    {
        CurrentBetText.text = "0";
        MaxBetText.text = "0";
        BottomPlayerNameText.text = "";
        TopPlayerNameText.text = "";
        BottomPlayerSprite.sprite = null;
        TopPlayerSprite.sprite = null;
    }

    private void SetProfilePicture(PlayerData Data, Image image, bool white)
    {
        if (Data == null || Data.Avatar == null)
        {
            image.sprite = white? Texture2D.whiteTexture.ToSprite() : Texture2D.blackTexture.ToSprite();
            Debug.LogError("SetImage PlayerData is Null");
            return;
        }

        SpriteData spriteData = Data.Avatar;
        spriteData.LoadImage(this, s => { image.sprite = s; }, AssetController.Instance.DefaultAvatar);
    }

    public void setCurrentBet(string bet)
    {
        CurrentBetText.text = bet;
    }

    public void OnShowLogsButton()
    {
        gameLogsView.SetActive(true);
    }
    #endregion Public Functions
}
