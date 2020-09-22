using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[Serializable]
public class ImageData : SpriteData
{
    public Color GradientColor1;
    public Color GradientColor2;

    public ImageData()
    {
        GradientColor1 = Color.white;
        GradientColor2 = Color.white;
    }

    public ImageData(Sprite sprite, Color color1, Color color2) : base(sprite)
    {
        GradientColor1 = color1;
        GradientColor2 = color2;
    }

    public ImageData(string pictureURL, Color color1, Color color2) : base(pictureURL)
    {
        GradientColor1 = color1;
        GradientColor2 = color2;
    }

    public ImageData(PopupImage popupImage)
    {
        Sprite = popupImage.image.sprite;
        PictureUrl = popupImage.pictureUrl;
        GradientColor1 = popupImage.gradient.startColor;
        GradientColor2 = popupImage.gradient.endColor;
    }

    public ImageData(Sprite sprite) : base(sprite)
    {
        GradientColor1 = Color.white;
        GradientColor2 = Color.white;
    }

    public ImageData(string pictureURL) : base(pictureURL)
    {
        GradientColor1 = Color.white;
        GradientColor2 = Color.white;
    }

    public ImageData(Color color1, Color color2)
    {
        GradientColor1 = color1;
        GradientColor2 = color2;
    }

    public ImageData(Dictionary<string, object> parameters, int DefaultSize = 30)
    {
        GradientColor1 = Color.white;
        GradientColor2 = Color.white;

        object o;
        if (parameters.TryGetValue("URL", out o))
            PictureUrl = o.ToString();

        if (parameters.TryGetValue("GradientColor1", out o))
        {
            List<float> layers = Utils.SplitToFloatList(o.ToString(), ',');
            GradientColor1 = new Color(layers[0], layers[1], layers[2], layers.Count > 3 ? layers[3] : 255);
        }

        if (parameters.TryGetValue("GradientColor2", out o))
        {
            List<float> layers = Utils.SplitToFloatList(o.ToString(), ',');
            GradientColor2 = new Color(layers[0], layers[1], layers[2], layers.Count > 3 ? layers[3] : 255);
        }
    }

}
