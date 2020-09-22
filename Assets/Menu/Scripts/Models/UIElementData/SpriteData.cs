using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;

[Serializable]
public class SpriteData
{
    public Sprite Sprite;
    public string PictureUrl;
    private IEnumerator loadImageRoutine;
#if !UNITY_WEBGL
    private bool SaveInLocal;
#endif

    protected SpriteData()
    {
    }

    public SpriteData(Sprite sprite)
    {
        Sprite = sprite;
    }

    public SpriteData(string pictureUrl, bool saveInLocal = false)
    {
#if !UNITY_WEBGL
        SaveInLocal = saveInLocal;
#endif
        PictureUrl = pictureUrl;
    }

    public void LoadImage(MonoBehaviour mono, UnityAction<Sprite> callback, Sprite defaultSprite = null)
    {
        if (loadImageRoutine != null)
            mono.StopCoroutine(loadImageRoutine);

        if (Sprite == null)
        {
            loadImageRoutine = LoadImageIEnumerator(defaultSprite, callback);
            mono.StartCoroutine(loadImageRoutine);
        }
        else if (callback != null)
            callback(Sprite);

    }

    public IEnumerator LoadImageIEnumerator(Sprite defaultSprite = null, UnityAction<Sprite> callback = null)
    {
        if (Sprite == null)
        {
            if(!string.IsNullOrEmpty(PictureUrl))
            {
#if !UNITY_WEBGL
                if(SaveInLocal)
                    yield return AssetController.Instance.GetStoreImage(PictureUrl, s => { Sprite = s; });
                else
#endif
                    yield return Utils.DownloadPic(PictureUrl, s => { Sprite = s; }, defaultSprite?? AssetController.Instance.EmptySprite);
            }
            else
                Sprite = defaultSprite;
        }

        if (callback != null)
            callback(Sprite);
    }

    public override string ToString()
    {
        return " sprite " + Sprite + " pictureUrl " + PictureUrl;
    }
}
