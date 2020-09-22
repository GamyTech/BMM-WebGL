using UnityEngine;
using UnityEngine.UI;

public class RatePopupWidget : Widget
{
    public Image Icon;
    public Text Title;
    public int HighestBadRating;
    public int StarsSelectedOnStar;
    public Transform StarsPanel;
    public Button SubmitButton;

    private int lastHovered;
    private int lastSelected;

    public override void EnableWidget()
    {
        base.EnableWidget();

        Icon.sprite = AppInformation.ICON;
        Title.text = string.Format(Utils.LocalizeTerm("Enjoying {0}?"), AppInformation.GAME_NAME);

        lastHovered = -1;
        lastSelected = StarsSelectedOnStar - 1;

        UpdateStars();

#if UNITY_WEBGL
        SubmitButton.RegisterCallbackOnPressedDown();
#endif
    }

    public override void DisableWidget()
    {
#if UNITY_WEBGL
        SubmitButton.UnregisterCallbackOnPressedDown();
#endif
        base.DisableWidget();
    }

    public void OnPointerEnter(int star)
    {
        if (lastHovered < star)
            lastHovered = star;
        UpdateStars();
    }

    public void OnPointerExit(int star)
    {
        if (lastHovered == star)
            lastHovered = -1;
        UpdateStars();
    }

    private void UpdateStars()
    {
        int size = StarsPanel.childCount;
        for (int i = 0; i < size; i++)
        {
            Transform star = StarsPanel.GetChild(i).GetChild(0);
            star.GetComponent<Text>().text = (lastHovered >= 0 ? i <= lastHovered : i <= lastSelected)? "ש" : "ת";
        }
    }

    public void SelectStar(int star)
    {
        lastSelected = star;
    }

    public void OnSubmitButton()
    {
        if (lastSelected >= 0)
        {
            if (lastSelected >= HighestBadRating)
                Utils.OpenURL(ContentController.GameRateSiteApple);
            else
                PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
        }
        CancelButton();
    }

    public void CancelButton()
    {
        WidgetController.Instance.HideWidgetPopups();
    }

}
