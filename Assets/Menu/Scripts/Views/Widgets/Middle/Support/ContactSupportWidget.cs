using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContactSupportWidget : Widget
{
    public void SupportButton()
    {
        PageController.Instance.CustomBackChangePage(Enums.PageId.ContactSupport,PageController.Instance.CurrentPageId);
    }
}
