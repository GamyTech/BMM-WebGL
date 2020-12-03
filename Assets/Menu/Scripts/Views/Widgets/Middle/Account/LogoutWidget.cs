using UnityEngine;
using System.Collections;

public class LogoutWidget : Widget {

    void Start()
    {
        if (NetworkController.Instance.IsClientGame())
        {
            gameObject.SetActive(false);
        }
    }

    public void LogOutButton()
    {
        PopupController.Instance.ShowSmallPopup("Are you sure you want to log out?", new SmallPopupButton("Yes", Logout), new SmallPopupButton("No"));
    }

    private void Logout()
    {
        UserController.Instance.Logout();
    }
}
