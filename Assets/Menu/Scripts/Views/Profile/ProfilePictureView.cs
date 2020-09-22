using UnityEngine;
using UnityEngine.UI;
using GT.Assets;
using System.Collections;

public class ProfilePictureView : MonoBehaviour
{
    public Image profilePictureImage;
    public Image luckyItemImage;

    public void SetProfilePic(Sprite profilePic)
    {
        if (profilePic != null)
            profilePictureImage.sprite = profilePic;
    }

    public IEnumerator SetProfilePicEnum(SpriteData picData, Sprite defaultTexture)
    {
        yield return picData.LoadImageIEnumerator(defaultTexture);
    }

    public void SetLuckyItem(Sprite luckyItem)
    {
        luckyItemImage.sprite = luckyItem != null? luckyItem : AssetController.Instance.EmptySprite;
    }
}
