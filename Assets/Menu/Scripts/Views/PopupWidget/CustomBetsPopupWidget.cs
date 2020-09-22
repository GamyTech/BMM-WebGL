using System;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.User;

public class CustomBetsPopupWidget : PopupWidget
{
    public ObjectPool BetPool;
    public RoomCollectionView RoomsView;
    public Text betHeadline;
    public SmoothLayoutElement betsSection;
    public Button selectButton;

    public override void EnableWidget()
    {
        base.EnableWidget();

        List<BetRoom> rooms = null;
        if (ContentController.CashRoomsByCategory.TryGetValue(ContentController.CustomCatId, out rooms))
            RoomsView.Init(BetRooms.CloneBetsList(rooms), true);
        else
            betHeadline.text = Utils.LocalizeTerm("No available bets");
    }

    public override void OnPopupWidgetShown()
    {
        betsSection.SetVerticalStateUsingLayoutSize(SmoothLayoutElement.State.Max);
    }

    private void SetButtons(bool cancel, bool play)
    {
        selectButton.transform.parent.gameObject.SetActive(play);
        selectButton.interactable = true;
    }

    #region Inputs
    public void SelectButton()
    {
        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
        user.UpdateBetCategory(ContentController.CustomCatId, RoomsView.rooms);
        SavedUsers.SaveUserToFile(user);

        CustomRoomCategoryView.ToUpdateSelected = true;
        WidgetController.Instance.HideWidgetPopups();
    }

    public void DeselectAllButton()
    {
        RoomsView.DeselectAll();
    }
    #endregion Inputs
}
