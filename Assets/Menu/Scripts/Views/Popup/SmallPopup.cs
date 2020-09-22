using UnityEngine.Events;

public class SmallPopup
{
    public UnityAction CloseCallBack { get; private set; }
    public string Headline { get; private set; }
    public string[] ContentText { get; private set; }
    public int BoldIndex { get; private set; }
    public SmallPopupButton[] Buttons { get; private set; }

    public SmallPopup(string headline, params SmallPopupButton[] buttons)
    {
        Headline = headline;
        Buttons = buttons;
    }

    public SmallPopup(string headline, string[] contentTexts, params SmallPopupButton[] buttons) : this(headline, buttons)
    {
        ContentText = contentTexts;
    }

    public SmallPopup(string headline, int boldIndex = 0, params SmallPopupButton[] buttons) : this(headline, buttons)
    {
        BoldIndex = boldIndex;
    }

    public SmallPopup(string headline, string[] contentTexts, int boldIndex = 0, params SmallPopupButton[] buttons) : this(headline, boldIndex, buttons)
    {
        ContentText = contentTexts;
    }

    public SmallPopup(string headline, string[] contentTexts, int boldIndex = 0, UnityAction callBack = null, params SmallPopupButton[] buttons) : this(headline, boldIndex, buttons)
    {
        ContentText = contentTexts;
        CloseCallBack = callBack;
    }
}
