using UnityEngine;
using GT.User;

public class LuckyItemsBarWidget : Widget
{
    public ProfilePictureView profilePictureView;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.gtUser.OnLuckyItemChanged += User_OnLuckyItemChanged;
        UserController.Instance.gtUser.OnAvatarChanged += User_OnAvatarChanged;

        profilePictureView.SetProfilePic(UserController.Instance.gtUser.Avatar);
        profilePictureView.SetLuckyItem(UserController.Instance.gtUser.LuckyItemSprite);
    }

    public override void DisableWidget()
    {
        UserController.Instance.gtUser.OnLuckyItemChanged -= User_OnLuckyItemChanged;
        UserController.Instance.gtUser.OnAvatarChanged -= User_OnAvatarChanged;

        base.DisableWidget();
    }

    private void User_OnAvatarChanged(Sprite newValue)
    {
        profilePictureView.SetProfilePic(newValue);
    }

    private void User_OnLuckyItemChanged(Sprite newValue)
    {
        profilePictureView.SetLuckyItem(newValue);
    }
}
