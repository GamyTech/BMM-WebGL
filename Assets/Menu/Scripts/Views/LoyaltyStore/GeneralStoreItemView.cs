using UnityEngine;
using UnityEngine.UI;
using GT.Store;
using UnityEngine.Events;

public class GeneralStoreItemView : MonoBehaviour, IStoreItemView
{
    public Text Name;
    public Image Image;
    public Button BuyButton;
    public Image buttonColor;
    public Sprite UsingButtonColor;
    public Sprite UseButtonColor;
    public Sprite BuyButtonColor;

    public string Id { get; private set; }

    protected RectTransform rectTransform { get { return transform as RectTransform; } }

    public virtual void Populate(StoreItem item)
    {
        Id = item.Id;
        Name.text = item.Name;
        SetImage(item.LocalSpriteData);
        SetButton(item);
    }

    void OnDestroy()
    {
        BuyButton.onClick.RemoveAllListeners();
    }

    protected void SetImage(SpriteData spriteData)
    {
        spriteData.LoadImage(this, s => Image.sprite = s, AssetController.Instance.DefaultImage);
    }

    protected virtual void SetButton(StoreItem item)
    {
        if(item.IsSelectable(1))
            PurchasedItemButtonHandler(item.SelectedString, item.Selected, item.Select);
        else
        {
            if (item.IsLocked())
                LockedItemButtonHandler(item.UnlockRank);
            else
                PrePurchasedItemButtonHandler(item.Cost.GetCostsString(), () => item.Purchase());
        }
    }

    protected void LockedItemButtonHandler(int unlockRank)
    {
        SetButtonText(Utils.LocalizeTerm("Unlocks at Rank {0}", unlockRank));
        BuyButton.onClick.RemoveAllListeners();
        BuyButton.interactable = false;
    }

    protected void PrePurchasedItemButtonHandler(string cost, UnityAction purchaseAction)
    {
        SetButtonText(cost);
        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(purchaseAction);
        buttonColor.sprite = BuyButtonColor;
    }

    protected void PurchasedItemButtonHandler(string selectText, bool isSelected, UnityAction selectAction)
    {
        SetButtonText(Utils.LocalizeTerm(selectText));
        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(selectAction);
        buttonColor.sprite = isSelected ? UsingButtonColor : UseButtonColor;
    }

    protected virtual void SetButtonText(string textValue)
    {
        Text text = BuyButton.GetComponentInChildren<Text>();
        text.text = textValue;
    }
}
