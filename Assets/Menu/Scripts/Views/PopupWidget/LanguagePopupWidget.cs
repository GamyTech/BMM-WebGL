using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguagePopupWidget : PopupWidget
{
    public ToggleGroup ToggleGroup;
    public ObjectPool Pool;

    Dictionary<string, LanguageToggle> languageTogglesDict = new Dictionary<string, LanguageToggle>();

    public override void EnableWidget()
    {
        base.EnableWidget();
        List<string> languages = I2.Loc.LocalizationManager.GetAllLanguages();
        languages.Sort();

        for (int i = 0; i < languages.Count; i++)
        {
            string language = languages[i];
            if (language == "Macedoniаn")
                continue;

            if (languageTogglesDict.ContainsKey(language))
                continue;

            GameObject go = Pool.GetObjectFromPool();
            go.InitGameObjectAfterInstantiation(Pool.transform);
            LanguageToggle toggle = go.GetComponent<LanguageToggle>();
            if (toggle != null)
            {
                languageTogglesDict.Add(language, toggle);
                if (I2.Loc.LocalizationManager.CurrentLanguage.Equals(language))
                    toggle.toggle.isOn = true;

                toggle.Populate(language, b => ChangeLanguage(b, language), ToggleGroup);
                ToggleGroup.RegisterToggle(toggle.toggle);
            }
            else
            {
                Debug.Log("Created object is does not have LanguageToggle component");
                Destroy(go);
            }
        }
    }

    #region Overrides
    public override void DisableWidget()
    {
        base.DisableWidget();

        foreach (var item in languageTogglesDict)
        {
            Pool.PoolObject(item.Value.gameObject);
        }
        languageTogglesDict.Clear();
    }
    #endregion Overrides

    #region Aid Functions
    private void ChangeLanguage(bool isOn, string language)
    {
        if (isOn == false)
            return;

        if (!string.IsNullOrEmpty(language))
        {
            if (!language.Equals(I2.Loc.LocalizationManager.CurrentLanguage))
            {
                I2.Loc.LocalizationManager.CurrentLanguage = language;
            }
            HidePopup();
        }
    }
    #endregion Aid Functions

}
