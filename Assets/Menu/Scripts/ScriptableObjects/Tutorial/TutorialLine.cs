using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialLine : MonoBehaviour
{
    public LayoutGroup layout;
    public GameObject IconContainer;
    public Text Message;
    public Image Icon;
    public Font BoldFont;

    public void Init(TutorialLineData data, int topPadding, int bottomPadding)
    {
        string finalMessage = data.Text;
        string Insert = string.Empty;
        if (finalMessage.Contains("["))
        {
            int start = finalMessage.IndexOf('[');
            Insert = finalMessage.Substring(start + 1, data.Text.Length - start - 2);
            finalMessage = start == 0 ? "" : finalMessage.Substring(0, start);
        }

        finalMessage = Utils.LocalizeTerm(finalMessage);
        Message.text = string.Format(finalMessage, Insert);
        layout.padding.top = topPadding;
        layout.padding.bottom = bottomPadding;

        if (data.Bold)
            Message.font = BoldFont;

        if (data.Icon != null)
        {
            layout.childAlignment = TextAnchor.MiddleLeft;
            Message.alignment = TextAnchor.MiddleLeft;
            Icon.sprite = data.Icon;
        }
        else
            IconContainer.SetActive(false);
    }
}

[Serializable]
public class TutorialLineData
{
    public string Text;
    public bool Bold;
    public Sprite Icon;
}
