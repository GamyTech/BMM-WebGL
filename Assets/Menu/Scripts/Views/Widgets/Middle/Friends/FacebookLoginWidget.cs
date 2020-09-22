using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookLoginWidget : Widget
{
    public override void EnableWidget()
    {
        base.EnableWidget();

        if (UserController.Instance.IsLoggedToFB())
            gameObject.SetActive(false);
        else
        {
            UserController.Instance.OnFacebookLoggedIn += OnFacebookLoggedIn;
            UserController.Instance.OnFacebookLoginAlreadyLoggedIn += OnFacebookLoggedIn;
            UserController.Instance.OnFacebookLoggedOut += OnFacebookLogFail;
            UserController.Instance.OnFacebookLoginCancelled += OnFacebookLogFail;
            UserController.Instance.OnFacebookLoginFailed += OnFacebookLogFail;
        }
    }

    public override void DisableWidget()
    {
        UserController.Instance.OnFacebookLoggedIn -= OnFacebookLoggedIn;
        UserController.Instance.OnFacebookLoginAlreadyLoggedIn -= OnFacebookLoggedIn;
        UserController.Instance.OnFacebookLoggedOut -= OnFacebookLogFail;
        UserController.Instance.OnFacebookLoginCancelled -= OnFacebookLogFail;
        UserController.Instance.OnFacebookLoginFailed -= OnFacebookLogFail;

        base.DisableWidget();
    }

    #region Events
    void OnFacebookLoggedIn()
    {
        LoadingController.Instance.HidePageLoading();
        gameObject.SetActive(false);
    }

    void OnFacebookLogFail()
    {
        LoadingController.Instance.HidePageLoading();
        gameObject.SetActive(true);
    }
    #endregion Events

    #region Buttons
    public void LoginToFacebook()
    {
        LoadingController.Instance.ShowPageLoading();
        UserController.Instance.FacebookLogin();
    }
    #endregion Buttons
}
