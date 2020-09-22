using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ButtonData
{
    public Enums.ActionType actionType;
    public string actionString;
    public TextData textData;
    public ImageData imageData;
    [NonSerialized]
    public UnityAction action;

    public ButtonData(Enums.ActionType actionType, string actionString, TextData textData, ImageData imageData)
    {
        this.actionType = actionType;
        this.actionString = actionString;
        this.textData = textData;
        this.imageData = imageData;
    }

    public ButtonData(UnityAction buttonAction, TextData textData, ImageData imageData)
    {
        this.action = buttonAction;
        this.textData = textData;
        this.imageData = imageData;
    }

    public ButtonData(PopupButton button) : this(button.actionType, button.actionString, new TextData(button.popupText), new ImageData(button.popupImage))
    {
    }

    public ButtonData(string text, UnityAction buttonAction)
    {
        textData = new TextData(text);
        textData.color = Color.black;
        imageData = new ImageData(Color.white, new Color(.5f, .5f, .5f));
        action = buttonAction;
    }


    public ButtonData(Dictionary<string, object> parameters)
    {
        object o;

        Enums.ActionType at = Enums.ActionType.None;
        if (parameters.TryGetValue("ActionType", out o) && Utils.TryParseEnum(o.ToString(), out at))
            actionType = at;

        actionString = parameters.TryGetValue("ActionString", out o) ? o.ToString() : "";

        if (parameters.TryGetValue("Text", out o))
            textData = new TextData(o as Dictionary<string, object>, 40);
        else
        {
            textData = new TextData("OK");
            textData.size = 40;
        }

        if (parameters.TryGetValue("Image", out o))
            imageData = new ImageData(o as Dictionary<string, object>);
        else
        {
            Color gradientColor1 = new Color(0.62f, 0.82f, 0.27f);
            Color gradientColor2 = new Color(0.08f, 0.51f, 0.17f);
            imageData = new ImageData(gradientColor1, gradientColor2);
        }
    }
}
