using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.User;

public class ProfileWidget : Widget
{

    public GameObject IndentityFlag;
    public GameObject ProFlag;

    public Image isActive;
    public Text UserName;
    public Image UserPicture;
    public Image UserLuckyItem;

    public Button AddFriendButton;
    public Button ChallengeButton;
    public Button ChangeProfileButton;

    public Image Medal;
    public List<GameObject> Stars = new List<GameObject>();
    public Text RankName;
    public Text Score;
    public Text Level;
    public Slider ProgressSlider;
    public Button SeeRanksButton;

    private GTUser m_SeenUser;

    public override void EnableWidget()
    {
        base.EnableWidget();
        Init();
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
        UserController.Instance.WatchedUser = null;
    }

    private void Init()
    {
        if (m_SeenUser != null)
            Unregiser();

        m_SeenUser = UserController.Instance.WatchedUser;
        if (m_SeenUser == null || m_SeenUser.Id == UserController.Instance.gtUser.Id)
        {
            m_SeenUser = UserController.Instance.gtUser;
            isActive.gameObject.SetActive(false);
            AddFriendButton.gameObject.SetActive(false);
            ChallengeButton.gameObject.SetActive(false);
            ChangeProfileButton.gameObject.SetActive(true);
            SeeRanksButton.gameObject.SetActive(true);

            Register();
        }

        else
        {
            isActive.gameObject.SetActive(true);
            AddFriendButton.gameObject.SetActive(true);
            ChallengeButton.gameObject.SetActive(true);
            ChangeProfileButton.gameObject.SetActive(false);
            SeeRanksButton.gameObject.SetActive(false);
        }

        //ProFlag.SetActive(false);
        UserName.text = m_SeenUser.UserName;
        if (m_SeenUser.Avatar != null)
            UserPicture.sprite = m_SeenUser.Avatar;
        UserLuckyItem.sprite = m_SeenUser.LuckyItemSprite;

        //Medal.sprite = m_SeenUser.rank.MedalTexture.ToSprite();
        //for (int x = 0; x < m_SeenUser.rank.Stars; ++x)
        //    Stars[x].SetActive(true);
        //for (int x = m_SeenUser.rank.Stars; x < Stars.Count; ++x)
        //    Stars[x].SetActive(false);
        //RankName.text = m_SeenUser.rank.MedalName;
        //Score.text = m_SeenUser.rank.Score.ToString("#.##0") + " Points";
        //Level.text = m_SeenUser.rank.Level.ToString();
        //ProgressSlider.value = m_SeenUser.rank.LevelProgress;

    }


    private void Register()
    {
        m_SeenUser.OnAvatarChanged += GtUser_OnAvatarChanged;
        m_SeenUser.OnLuckyItemChanged += GtUser_OnLuckyItemChanged;
        //m_SeenUser.rank.OnScoreChanged += Rank_OnScoreChanged;
    }

    private void Unregiser()
    {
        m_SeenUser.OnAvatarChanged -= GtUser_OnAvatarChanged;
        m_SeenUser.OnLuckyItemChanged -= GtUser_OnLuckyItemChanged;
        //m_SeenUser.rank.OnScoreChanged -= Rank_OnScoreChanged;
    }


    private void GtUser_OnLuckyItemChanged(Sprite newValue)
    {
        UserLuckyItem.sprite = newValue;
    }

    private void GtUser_OnAvatarChanged(Sprite newValue)
    {
        if (UserPicture != null)
            UserPicture.sprite = newValue;
    }

    //private void Rank_OnScoreChanged(int newValue)
    //{
    //    Medal.sprite = m_SeenUser.rank.MedalTexture.ToSprite();
    //    for (int x = 0; x < m_SeenUser.rank.Stars; ++x)
    //        Stars[x].SetActive(true);
    //    for (int x = m_SeenUser.rank.Stars; x < Stars.Count; ++x)
    //        Stars[x].SetActive(false);
    //    RankName.text = m_SeenUser.rank.MedalName;
    //    Score.text = m_SeenUser.rank.Score.ToString("#.##0") + " Points";
    //    Level.text = m_SeenUser.rank.Level.ToString();
    //    ProgressSlider.value = (float)m_SeenUser.rank.Score / (float)m_SeenUser.rank.LevelProgress;
    //}

    #region Buttons

    public void AddFriend()
    {

    }

    public void Challenge()
    {

    }

    public void ChangeProfile()
    {
        PageController.Instance.ChangePage(Enums.PageId.ChangeProfile);
    }

    public void SeeRanks()
    {
        PageController.Instance.ChangePage(Enums.PageId.Rank);
    }

    #endregion Buttons
}
