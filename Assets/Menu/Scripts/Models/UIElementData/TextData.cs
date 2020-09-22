using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TextData
{
    public string text = "";
    public int size = 20;
    public Color color = Color.white;
    public FontStyle style = FontStyle.Normal;
    public TextAnchor anchor = TextAnchor.MiddleCenter;
    public string fontName;
    public float lineSpacing = 1;
    public float flexible = 1;

    public TextData(PopupText textElement)
    {
        if (textElement == null)
            return;
        text = textElement.text.text;
        size = textElement.text.fontSize;
        color = textElement.text.color;
        style = textElement.text.fontStyle;
        anchor = textElement.text.alignment;
        fontName = textElement.text.font.name; // Might not work
        lineSpacing = textElement.text.lineSpacing;
        flexible = textElement.layoutElement.flexibleHeight;
    }

    public TextData(string text)
    {
        this.text = text;
    }

    public TextData(Dictionary<string, object> parameters, int DefaultSize = 30)
    {
        object o;
        if (parameters.TryGetValue("Text", out o))
        {
            text = o.ToString();

            size = parameters.TryGetValue("Size", out o) ? o.ParseInt() : DefaultSize;

            lineSpacing = parameters.TryGetValue("LineSpacing", out o) ? o.ParseFloat() : 1;

            fontName = parameters.TryGetValue("Font", out o) ? o.ToString() : "GTHelveticaNeueMedium";

            if (parameters.TryGetValue("Color", out o))
            {
                List<float> layers = Utils.SplitToFloatList(o.ToString(), ',');
                color = new Color(layers[0], layers[1], layers[2], layers.Count > 3 ? layers[3] : 255);
            }
            else
                color = Color.white;

            FontStyle s;
            if (parameters.TryGetValue("Style", out o) && Utils.TryParseEnum(o.ToString(), out s))
                style = s;
            else
                style = FontStyle.Normal;
        }
    }
}
