using UnityEngine.UI;

public class PopupWidget : Widget, IResizable
{
    public Button CloseButton;
    public bool IsClosable { get; protected set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (CloseButton != null)
            CloseButton.onClick.AddListener(HidePopup);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (CloseButton != null)
            CloseButton.onClick.RemoveListener(HidePopup);
    }

    public virtual void Init(bool closable = true, object data = null)
    {
        IsClosable = closable;
    }

    protected virtual void HidePopup()
    {
        WidgetController.Instance.HideWidgetPopup(this);
    }

    public virtual void OnPopupWidgetShown() { }
}
