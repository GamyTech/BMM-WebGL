using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GT.Store;
using GT.User;

public class ShoutOutWidget : Widget
{
    private VerticalDynamicContentLayoutGroup m_content;
    public VerticalDynamicContentLayoutGroup content
    {
        get
        {
            if (m_content == null)
                m_content = GetComponentInChildren<VerticalDynamicContentLayoutGroup>();
            return m_content;
        }
    }

    public delegate void Changed<T>(T newValue);
    public static event Changed<string> ShoutOutClickedEvent;

    public static void ShoutOutClicked(string shoutOut)
    {
        SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
        //user.currentShoutOut = shoutOut;
        SavedUsers.SaveUserToFile(user);

        if (ShoutOutClickedEvent != null)
            ShoutOutClickedEvent(shoutOut);
    }

    public override void EnableWidget()
    {
        base.EnableWidget();
        InitList(UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.ShoutOuts).items);
    }

    public override void DisableWidget()
    {
        content.ClearElements();

        base.DisableWidget();
    }

    public void InitList(List<StoreItem> item)
    {
        content.SetElements(new List<IDynamicElement>(item.ToArray()));
    }

    void SetItems(List<IDynamicElement> items)
    {
        content.SetElements(items);
    }
}
