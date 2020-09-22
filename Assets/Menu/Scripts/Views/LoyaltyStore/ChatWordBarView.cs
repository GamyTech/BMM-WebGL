using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GT.Store;

public class ChatWordBarView : MonoBehaviour, IStoreItemView
{
    private const string ANIMATION_SELECTED = "isSelected";
    private const string ANIMATION_SHAKING = "isShaking";

    public delegate void ButtonTriggered(ChatWordBarView view);
    public event ButtonTriggered OnButtonTriggered;

    public Text text;
    public Image image;
    public Button BuyButton;
    public Animator animatior;

    public StoreItem Item { get; protected set; }

    private IEnumerator SetImageIEnumerator;

    public virtual void Populate(StoreItem item)
    {
        Item = item;
        FillInfo(Item);
        SetButton(Item);

        AnimateSelection(false);
    }

    public void AnimateShaking(bool isOn)
    {
        if (animatior.GetBool(ANIMATION_SHAKING) != isOn)
        {
            if (isOn)
                Invoke("AnimateShakingOn", UnityEngine.Random.Range(0, 0.25f));
            else
                animatior.SetBool(ANIMATION_SHAKING, false);
        }
    }

    public void AnimateShakingOn()
    {
        animatior.SetBool(ANIMATION_SHAKING, true);
    }

    public void AnimateSelection(bool isSelected)
    {
        if (animatior.GetBool(ANIMATION_SELECTED) != isSelected)
            animatior.SetBool(ANIMATION_SELECTED, isSelected);
    }

    protected void FillInfo(StoreItem item)
    {
        if (item.LocalSpriteData == null || string.IsNullOrEmpty((string)item.LocalSpriteData.PictureUrl))
        {
            text.enabled = true;
            image.enabled = false;
            text.text = item.Name;
        }
        else
        {
            text.enabled = false;
            image.enabled = true;
            SetImage(item.LocalSpriteData);
        }
    }

    protected void SetImage(SpriteData spriteData)
    {
        spriteData.LoadImage(this, s => image.sprite = s, AssetController.Instance.DefaultImage);
    }

    protected virtual void SetButton(StoreItem item)
    {
        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (OnButtonTriggered != null)
            OnButtonTriggered(this);
    }
}
