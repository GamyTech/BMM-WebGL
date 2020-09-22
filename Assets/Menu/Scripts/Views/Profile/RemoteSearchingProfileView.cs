using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RemoteSearchingProfileView : SearchingProfilePictureView
{
    private const float epsilon = 0.001f;
    private const float waitTime = 0.5f;

    public RawImage spiningImage;
    public float spinSpeed = .01f;
    public bool UseGenericUser = true;
    public string GenericUserName;

    bool isSpining;
    bool isStopping;

    float currentPos = 0;
    float targetPos = -200;
    float velocity = 0;
    float moveTime = 0.1f;

    void FixedUpdate()
    {
        if (isSpining)
        {
            Rect uvRect = spiningImage.uvRect;
            uvRect.y += spinSpeed;
            spiningImage.uvRect = uvRect;
        }

        if(isStopping)
        {
            if (Utils.Approximately(currentPos, targetPos, epsilon) == false)
            {
                currentPos = Mathf.SmoothDamp(currentPos, targetPos, ref velocity, moveTime);
                (profilePictureImage.transform as RectTransform).anchoredPosition = new Vector2(0, currentPos);
            }
            else if(currentPos != targetPos)
            {
                currentPos = targetPos;
            }
            else
            {
                isStopping = false;
                isSpining = false;
            }
        }
    }
    
    public void StartSpining()
    {
        MenuSoundController.Instance.Play(Enums.MenuSound.Spinning);

        currentPos = 0;
        (profilePictureImage.transform as RectTransform).anchoredPosition = Vector2.zero;

        SetTexts("");
        SetLuckyItem(null);

        isStopping = false;
        isSpining = true;
    }

    public void StopSpining(string name, SpriteData pic, Sprite luckyItem)
    {
        if(UseGenericUser)
        {
            name = GenericUserName;
            pic = null;
            luckyItem = null;
        }
        StartCoroutine(SetUserData(name, pic, luckyItem));
    }

    private IEnumerator SetUserData(string name, SpriteData pic, Sprite luckyItem)
    {
        if (pic != null)
            yield return StartCoroutine(SetProfilePicEnum(pic, AssetController.Instance.DefaultAvatar));
        else
            SetProfilePic(AssetController.Instance.DefaultAvatar);

        isStopping = true;
        SetTexts(name);
        SetLuckyItem(luckyItem);
    }
}
