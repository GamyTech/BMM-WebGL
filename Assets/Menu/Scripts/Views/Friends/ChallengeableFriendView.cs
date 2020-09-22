using UnityEngine;
using UnityEngine.UI;

public class ChallengeableFriendView : MonoBehaviour
{
    public Text originText;
    public Image profilePic;
    public Image statusIndicator;
    public Text nameText;
    public Button playButton;

    private ChallengeableFriend friend;

    public void PopulateData(ChallengeableFriend friend)
    {
        this.friend = friend;
        RedrawData();
    }

    public void OnStatusChanged(ChallengeableFriend.FriendStatus status)
    {
        SetState(status);
    }

    private void RedrawData()
    {
        nameText.text = friend.Name;
        originText.text = FBUser.OriginToCharDict[friend.Origin].ToString();
        SetImage(friend.Picture);
        SetState(friend.Status);
    }

    private void SetState(ChallengeableFriend.FriendStatus status)
    {
        playButton.interactable = status == ChallengeableFriend.FriendStatus.Online;
        statusIndicator.color = FBUser.StatusToColorDict[friend.Status];
    }

    private void SetImage(SpriteData data)
    {
        data.LoadImage(this, s => { profilePic.sprite = s; }, AssetController.Instance.DefaultAvatar);
    }
    #region Input
    public void OnPlayClicked()
    {
        friend.Play();
    }
    #endregion Input
}
