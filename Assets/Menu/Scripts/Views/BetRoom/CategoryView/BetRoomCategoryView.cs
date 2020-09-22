using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.User;

public abstract class BetRoomCategoryView : MonoBehaviour
{
    public Text GameNameText;
    public Text LeftSignText;
    public Text RightSignText;

    protected List<BetRoom> Rooms;
    public RoomCategory Category { get; private set; }
    public Transform PlayButton;
    private string CategoryId;

    public void Init(string catId, RoomCategory category, List<BetRoom> rooms)
    {
        Category = category;
        CategoryId = catId;
        Rooms = BetRooms.CloneBetsList(rooms);

        if (Rooms == null || Rooms.Count == 0)
        {
            Debug.LogError("category " + Category.Name + "has no rooms");
            return;
        }
        Rooms.Sort((a, b) => a.WinAmount.CompareTo(b.WinAmount));

        SetTitle();
        InitRooms();
    }

    protected abstract void InitRooms();

    protected virtual void SetTitle()
    {
        GameNameText.text = Utils.LocalizeTerm(Category.Name);
        LeftSignText.text = Category.Sign;
        RightSignText.text = Category.Sign;
    }

    #region Buttons
    public virtual void Play()
    {
        List<BetRoom> selectedRooms = new List<BetRoom>();
        for (int x = 0; x < Rooms.Count; ++x)
            if(Rooms[x].Selected)
                selectedRooms.Add(Rooms[x]);

        if (selectedRooms.Count == 0)
        {
            PopupController.Instance.ShowSmallPopup("Please Select at least one bet");
            return;
        }
        if (!UserController.Instance.wallet.HaveEnoughMoney(selectedRooms))
        {
            PopupController.Instance.ShowSmallPopup("You dont have enough money to play on selected amounts");
            return;
        }
        if (UserController.Instance.CheckAndShowUserVerification())
            return;

        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
        user.UpdateBetCategory(CategoryId, Rooms);
        SavedUsers.SaveUserToFile(user);

        UserController.Instance.SendStartSearchingMatch(selectedRooms);
        PageController.Instance.ChangePage(Enums.PageId.Searching);
    }
    #endregion Buttons
}
