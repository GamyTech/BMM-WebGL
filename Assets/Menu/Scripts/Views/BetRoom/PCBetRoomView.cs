using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PCBetRoomView : BetRoomView
{
    public List<Sprite> BetDiceLogosRef = new List<Sprite>();
    public Image toggleBackground;
    public Image toggleCheckMark;

    public override void Populate(BetRoom room, ToggleGroup group, UnityAction OnChangedAction = null)
    {
        base.Populate(room, group, OnChangedAction);

        if (!UserController.Instance.wallet.HaveEnoughMoney(room))
        {
            room.Selected = false;
            SelectedToggle.interactable = false;
            toggleCheckMark.gameObject.SetActive(false);
            toggleBackground.gameObject.SetActive(false);
        }
        else
        {
            Sprite RoomLogo = GetDiceLogoRooms(room.BetAmount, AppInformation.MATCH_KIND);
            toggleBackground.sprite = RoomLogo;
            toggleCheckMark.sprite = RoomLogo;
        }
    }

    protected override void SetBetText(BetRoom room)
    {
        BetAmountText.text = Wallet.AmountToString(room.BetAmount, room.Kind);
    }
    protected override void SetWinAndLoyaltyTexts(BetRoom room) {}

    public Sprite GetDiceLogoRooms(float bet, Enums.MatchKind kind)
    {
        List<BetRoom> rooms;
        ContentController.GetByCategory(AppInformation.MATCH_KIND).TryGetValue(ContentController.CustomCatId, out rooms);
        int minLenght = Mathf.Min(rooms.Count, BetDiceLogosRef.Count); 
        int x = 0;
        while (x < minLenght && rooms[x].BetAmount != bet)
        {
            ++x;
        }

        if (x < minLenght)
            return BetDiceLogosRef[x];
        else
        {
            Debug.LogWarning("Bet out of Dices Logo Area, Using Last Icon");
            return BetDiceLogosRef[BetDiceLogosRef.Count - 1];
        }
    }

}
