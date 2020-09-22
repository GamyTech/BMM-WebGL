using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using GT.Store;

public class ChatImageButtonView : MonoBehaviour
{
    public Image image;
    public Text text;

    public void Init(StoreItem data, UnityAction<string> action)
    {
        if (data.LocalSpriteData != null)
            data.LocalSpriteData.LoadImage(this, t => { image.sprite = t; });
        GetComponent<Button>().onClick.AddListener(()=>action(data.Id));
    }
}
