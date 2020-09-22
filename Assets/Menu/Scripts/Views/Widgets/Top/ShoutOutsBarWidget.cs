using UnityEngine.UI;

public class ShoutOutsBarWidget : Widget
{
    public RawImage UserPicture;
    public Text CurrentShoutOut;

    protected override void OnEnable()
    {
        base.OnEnable();
        ShoutOutWidget.ShoutOutClickedEvent += ShoutOutWidget_ShoutOutClickedEvent;
        Initialize();
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
        ShoutOutWidget.ShoutOutClickedEvent -= ShoutOutWidget_ShoutOutClickedEvent;

    }

    private void ShoutOutWidget_ShoutOutClickedEvent(string newValue)
    {
        CurrentShoutOut.text = newValue;
    }

    public void Initialize()
    {
        if (UserController.Instance.gtUser == null) return;

        //SavedUser user = SavedUsers.LoadUserFromFile(UserController.Instance.gtUser.Id);
        //CurrentShoutOut.text = user.currentShoutOut;

        if (UserController.Instance.gtUser.Avatar != null)
        {
            UserPicture.texture = UserController.Instance.gtUser.Avatar.texture;
        }
    }

}
