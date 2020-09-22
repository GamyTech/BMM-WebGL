using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BetRoomView : MonoBehaviour
{
    public Toggle SelectedToggle;
    public Text BetAmountText;
    public Text FeeAmountText;
    public Text WinAmountText;

    protected BetRoom m_room;
    private UnityAction OnChangedAction;

    public virtual void Populate(BetRoom room, ToggleGroup group, UnityAction onChangedAction = null)
    {
        this.m_room = room;

        OnChangedAction = onChangedAction;
        SelectedToggle.group = group;

        SetBetText(m_room);
        SetFeeText(m_room);
        SetWinAndLoyaltyTexts(m_room);
    }

    protected virtual void SetBetText(BetRoom room)
    {
        BetAmountText.text = Wallet.AmountToString(room.BetAmount, room.Kind);
    }

    protected virtual void SetFeeText(BetRoom room)
    {
        FeeAmountText.text = "+" + Utils.LocalizeTerm("Fee") + " " + Wallet.AmountToString(room.FeeAmount, room.Kind);
    }

    protected virtual void SetWinAndLoyaltyTexts(BetRoom room)
    {
        WinAmountText.text = Wallet.AmountToString(room.WinAmount, room.Kind);
    }

    public virtual void RefreshSelected()
    {
        SelectedToggle.isOn = m_room.Selected;
    }

    public void OnToggleValueChanged(bool isOn)
    {
        m_room.Selected = isOn;
        if(OnChangedAction != null)
            OnChangedAction();
    }
}
