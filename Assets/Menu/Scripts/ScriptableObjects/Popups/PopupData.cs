using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable, CreateAssetMenu(menuName = "Assets/Popup Data")]
public class PopupData : ScriptableObject
{
    public string popupId;
    public TextData headline;
    public ImageData picture;
    public bool isCloseable;
    public bool isStatic;
    public List<TextData> textElements;
    public List<ButtonData> buttonElements;
    public AudioClip startSound;

    public PopupData(Enums.PopupId type, TextData headline, ImageData picture, bool isClosable, List<TextData> content, List<ButtonData> buttons, bool isStatic)
    {
        SetData(type, headline, picture, isClosable, content, buttons, isStatic);
    }

    public void SetData(Enums.PopupId type, TextData headline, ImageData picture, bool isClosable, List<TextData> content, List<ButtonData> buttons, bool isStatic)
    {
        this.popupId = type.ToString();
        this.headline = headline;
        this.picture = picture;
        this.isCloseable = isClosable;
        this.isStatic = isStatic;
        this.textElements = content;
        this.buttonElements = buttons;
    }
}
