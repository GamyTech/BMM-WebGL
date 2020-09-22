using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GT.Store;

public class ShoutOutView : MonoBehaviour, IStoreItemView
{

    public Text ShoutOutText;
    public Text PriceText;
    public Text IconText;
    public Button Button;


    private FadingElement fadingElement;
    private FadingElement FadingElement
    {
        get
        {
            if (fadingElement == null)
                fadingElement = GetComponent<FadingElement>();
            return fadingElement;
        }
    }

    public void Populate(StoreItem item)
    {
        ShoutOutText.text = item.Name;
        Button.onClick.RemoveAllListeners();
        if (item is IOwnable)
        {
            IOwnable ownable = (IOwnable)item;
            if (ownable.Owned)
            {
                IconText.text = "¥";
                PriceText.text = string.Empty;
                Button.interactable = !item.Selected;
                Button.onClick.AddListener(() => UseShoutOut(item));
                IconText.GetComponent<UnityEngine.UI.Gradient>().startColor = new Color(0f, 0.5254f, 0.7647f);
                IconText.GetComponent<UnityEngine.UI.Gradient>().endColor = new Color(0f, 0.3803f, 0.5333f);
            }
            else
            {
                IconText.text = ".";
                PriceText.text = item.Cost.GetCostsString();
                Button.onClick.AddListener(() => item.Purchase());
                IconText.GetComponent<UnityEngine.UI.Gradient>().startColor = Color.grey;
                IconText.GetComponent<UnityEngine.UI.Gradient>().endColor = Color.grey;
            }

        }
        else if (item is IConsumable)
        {

        }
    }

    private void UseShoutOut(StoreItem item)
    {
        item.Select();
        StartCoroutine(FadeInOut());
    }

    IEnumerator FadeInOut()
    {
        FadingElement.FadeOut();
        yield return new WaitForSeconds(0.3f);
        FadingElement.FadeIn();
    }
}
