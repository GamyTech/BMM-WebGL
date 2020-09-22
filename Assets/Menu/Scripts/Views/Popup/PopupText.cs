using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupText : MonoBehaviour
{
    public Text text;
    public LayoutElement layoutElement;


    public void SetText(TextData textData)
    {
        if (textData == null)
            return;
        text.text = textData.text;
        text.fontSize = textData.size;
        text.color = textData.color;
        text.fontStyle = textData.style;
        text.alignment = textData.anchor;
        // TODO get font by name
        //text.font = textData.fontName;
        text.lineSpacing = textData.lineSpacing;
        layoutElement.flexibleHeight = textData.flexible;
    }

    public void Reset()
    {
        text.text = "";
        text.fontSize = 30;
        text.color = Color.black;
        text.fontStyle = FontStyle.Normal;
        text.alignment = TextAnchor.MiddleCenter;
        // TODO get font by name
        //text.font = default font;
        text.lineSpacing = 1;
        layoutElement.flexibleHeight = 1;
    }
}