using GT.Websocket;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationWidget : Widget
{
    public Text versionField;
    public Text usernameField;
    public Text emailField;
    public Text ConnectionField;

    public override void EnableWidget()
    {
        base.EnableWidget();

        versionField.text = Utils.LocalizeTerm("Version") + ": " + AppInformation.BUNDLE_VERSION;
        usernameField.text = Utils.LocalizeTerm("Username") + ": " + UserController.Instance.gtUser.UserName;
        emailField.text = Utils.LocalizeTerm("Email") + ": " + UserController.Instance.gtUser.Email;

        bool NotReleasedBuild = NetworkController.Instance.Connection != NetworkController.ServerType.GlobalServer;
        ConnectionField.gameObject.SetActive(NotReleasedBuild);
        if (NotReleasedBuild)
            ConnectionField.text = "Connection : " + NetworkController.Instance.Connection.ToString();
    }
}
