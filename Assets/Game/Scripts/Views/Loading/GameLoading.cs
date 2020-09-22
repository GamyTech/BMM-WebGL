using UnityEngine;
using UnityEngine.UI;

public class GameLoading : SmallLoadingView
{
    public Text text;

    public void ShowLoading(string text, bool isInstant = false)
    {
        this.text.text = text;
        base.ShowLoading(transform as RectTransform, null, isInstant);
    }

    public void HideLoading(bool isInstant = false)
    {
        base.HideLoading(null, isInstant);
    }
}
