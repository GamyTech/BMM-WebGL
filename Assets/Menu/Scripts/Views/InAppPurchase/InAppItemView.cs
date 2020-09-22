using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GT.InAppPurchase;
using UnityEngine.Events;

public class InAppItemView : MonoBehaviour
{
    public Text packageName;
    public Text item;
    public Text price;
    public Image image;
    public Button button;

    public void Populate(InAppData inAppData, UnityAction action)
    {
        if (inAppData.imageData == null)
            return;
        inAppData.imageData.LoadImage(this, s => { image.sprite = s; });

        packageName.text = Utils.LocalizeTerm(inAppData.name);
        item.text = Wallet.VirtualPostfix + Wallet.AmountToStringStartingFromExponent(6, inAppData.value, 1);
        price.text = Wallet.CashPostfix + inAppData.price;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
