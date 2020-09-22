using UnityEngine;
using GT.Backgammon.Player;
using System.Collections.Generic;

public class MascotsView : MonoBehaviour
{
    public MascotView UserPlayerMascot;
    public MascotView OpponentPlayerMascot;

    private IPlayer userPlayer;
    private PlayerData opponentData;

    public void InitPlayersData(IPlayer userPlayer, IPlayer opponentPlayer)
    {
        this.userPlayer = userPlayer;
        opponentData = opponentPlayer.playerData;
        userPlayer.playerData.OnSelectedItemsUpdate += PlayerData_OnPlayerSelectedItemsUpdate;
        opponentData.OnSelectedItemsUpdate += PlayerData_OnOpponentSelectedItemsUpdate;

        UpdateMascotView(UserPlayerMascot, userPlayer.playerData.SelectedItems);
        UpdateMascotView(OpponentPlayerMascot, opponentPlayer.playerData.SelectedItems);
    }

    private void UpdateMascotView(MascotView mascot, Dictionary<Enums.StoreType, string[]> selectedItems)
    {
        string[] mascotID = null;
        if (selectedItems.TryGetValue(Enums.StoreType.Mascots, out mascotID) && mascotID.Length > 0)
        {
            UpdateMascotViewById(mascot, mascotID[0]);
        }
        else
        {
            mascot.RemoveMascot();
        }
    }

    private void UpdateMascotViewById(MascotView mascot, string selectedItem)
    {
        mascot.Init(selectedItem);
    }

    private void PlayerData_OnPlayerSelectedItemsUpdate(Dictionary<Enums.StoreType, string[]> selectedItems)
    {
        UpdateMascotView(UserPlayerMascot, selectedItems);
    }

    private void PlayerData_OnOpponentSelectedItemsUpdate(Dictionary<Enums.StoreType, string[]> selectedItems)
    {
        UpdateMascotView(OpponentPlayerMascot, selectedItems);
    }

    void OnDestroy()
    {
        if (opponentData != null)
            opponentData.OnSelectedItemsUpdate -= PlayerData_OnOpponentSelectedItemsUpdate;
    }
}
