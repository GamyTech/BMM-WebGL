using UnityEngine;
using UnityEngine.Events;

public class ActionKit
{
    public static UnityAction<bool> CreateActionBool(Enums.ActionType type, string actionString)
    {
        UnityAction<bool> returnAction = null;
        switch (type)
        {
            case Enums.ActionType.ChangePref:
                Enums.PlayerPrefsVariable key;
                if (Utils.TryParseEnum(actionString, out key))
                    returnAction = b => GTDataManagementKit.SaveToPlayerPrefs(key, b.ToString());
                else
                    Debug.LogError("Unknow player prefs key : " + actionString);
                break;
        }
        return returnAction;
    }

    public static UnityAction CreateAction(Enums.ActionType type, string actionString, out bool OpenWebPage)
    {
        OpenWebPage = false;
        UnityAction returnAction = null;
        switch (type)
        {
            case Enums.ActionType.OpenUrl:
                OpenWebPage = true;
                returnAction = () => Utils.OpenURL(actionString);
                break;
            case Enums.ActionType.OpenPage:
                Enums.PageId page;
                if (Utils.TryParseEnum(actionString, out page, true))
                    returnAction = () => { PageController.Instance.ChangePage(page); };
                break;
            case Enums.ActionType.OpenUpdatePage:
                OpenWebPage = true;
                string site;
#if UNITY_ANDROID
                site = ContentController.GameSiteGoogle;
#elif UNITY_IOS
                site = ContentController.GameSiteApple;
#else
                site = ContentController.GameWebsite;
#endif
                returnAction = () => Utils.OpenURL(site, true);
                break;
            case Enums.ActionType.OpenTermsOfUse:
                OpenWebPage = true;
                returnAction = () => Utils.OpenURL(ContentController.TermsOfUseSite, "Terms Of Use");
                break;
            case Enums.ActionType.OpenPrivacyPolicy:
                OpenWebPage = true;
                returnAction = () => Utils.OpenURL(ContentController.PrivacyPolicySite, "Privacy Policy");
                break;
            case Enums.ActionType.CloseApp:
#if UNITY_EDITOR
                returnAction = () => UnityEditor.EditorApplication.isPlaying = false;
#else
                returnAction = () => Application.Quit();
#endif
                break;
            case Enums.ActionType.ShowPopup:
                Enums.WidgetId popup;
                if (Utils.TryParseEnum(actionString, out popup, true))
                    returnAction = () => { WidgetController.Instance.ShowWidgetPopup(popup); };
                break;
        }
        return returnAction;
    }

    public static UnityAction CreateAction(string type, string actionString, out bool OpenWebPage)
    {
        UnityAction returnAction = null;
        OpenWebPage = false;
        Enums.ActionType aType;
        if(Utils.TryParseEnum(type, out aType))
            returnAction = CreateAction(aType, actionString, out OpenWebPage);
        else
            Debug.LogError("Unrecognized action type");
        return returnAction;
    }
}
