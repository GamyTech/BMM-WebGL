using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class PopupImage : MonoBehaviour
{
    public string pictureUrl;
    public Image image;
    public UnityEngine.UI.Gradient gradient;

    public void LoadImageData(ImageData data, MonoBehaviour mono, UnityAction callback = null)
    {
        gradient.startColor = data.GradientColor1;
        gradient.endColor = data.GradientColor2;
        pictureUrl = data.PictureUrl;

       data.LoadImage(mono, t => { image.sprite = t; if (callback != null) callback(); });
    }

    public void Reset()
    {
        image.sprite = null;
        pictureUrl = null;
        gradient.startColor = Color.black;
        gradient.endColor = Color.white;
    }
}
