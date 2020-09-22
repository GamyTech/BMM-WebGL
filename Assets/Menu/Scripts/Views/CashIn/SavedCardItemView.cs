using UnityEngine;
using UnityEngine.UI;

public class SavedCardItemView : MonoBehaviour
{
    public delegate void StateChange(GT.User.GTUser.SavedCreditCard card);
    public StateChange OnSelected = (i) => { };
    public StateChange OnDelete = (i) => { };

    public Image CardType;
    public Text LastDigits;
    public Image SelectedIcon;
    public Toggle SelectionToggle;
    public SmoothResize DeleteButtonResizer;
    public float DeleteButtonWidth;

    private GT.User.GTUser.SavedCreditCard card;

    public void Populate(ToggleGroup group, GT.User.GTUser.SavedCreditCard card)
    {
        SelectionToggle.group = group;

        this.card = card;
        UpdateTypeIcon();
        LastDigits.text = card.lastDigits;
        SetSelected(false);

        ToggleEdit(false);
    }

    public void ToggleEdit(bool isEditMode)
    {
        DeleteButtonResizer.targetWidth = isEditMode ? DeleteButtonWidth : 0;
    }

    private void UpdateTypeIcon()
    {
        Sprite sprite = null;
        AssetController.Instance.CreditCard.TryGetValue(card.cardType, out sprite);
        CardType.sprite = sprite;
        CardType.transform.parent.gameObject.SetActive(sprite != null);
    }

    private void SetSelected(bool isSelected)
    {
        SelectedIcon.enabled = isSelected;
    }

    #region Input
    public void OnSelect()
    {
        if (SelectionToggle.isOn)
            OnSelected(card);
        SetSelected(SelectionToggle.isOn);
    }

    public void Delete()
    {
        OnDelete(card);
    }
    #endregion Input
}
