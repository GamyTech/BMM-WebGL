using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionPopupWidget : PopupWidget
{

    public ToggleGroup toggleGroup;
    public ObjectPool pool;
    private bool m_manualChanging = true;
    private int m_actualResToggle = 0;

    List<ResolutionToggle> ResolutionToggles = new List<ResolutionToggle>();

    public override void EnableWidget()
    {
        base.EnableWidget();
        Populate();
        SettingsController.OnScreenSizeChanged += SettingsController_OnScreenSizeChange;
    }

    private void Populate()
    {
        m_actualResToggle = SettingsController.currentResIndex == -1 ? ResolutionToggles.Count - 1 : SettingsController.currentResIndex;
        for (int i = 0; i < SettingsController.FixedResolutions.Count; i++)
        {
            string resolutionName = SettingsController.FixedResolutions[i].x.ToString() + "x" + SettingsController.FixedResolutions[i].y.ToString();

            GameObject go = pool.GetObjectFromPool();
            go.InitGameObjectAfterInstantiation(pool.transform);
            ResolutionToggle toggle = go.GetComponent<ResolutionToggle>();
            if (toggle != null)
            {
                int x = i;
                toggle.toggle.isOn = i == m_actualResToggle;
                toggle.Populate(resolutionName, b => ChangeResolution(b, x), toggleGroup);
                toggleGroup.RegisterToggle(toggle.toggle);
                ResolutionToggles.Add(toggle);
            }
            else
            {
                Debug.Log("Created object is does not have Resolution Toggle component");
                Destroy(go);
            }
        }

        GameObject lastGo = pool.GetObjectFromPool();
        lastGo.InitGameObjectAfterInstantiation(pool.transform);
        ResolutionToggle lastToggle = lastGo.GetComponent<ResolutionToggle>();
        lastToggle.toggle.isOn = SettingsController.currentResIndex == -1;
        ResolutionToggles.Add(lastToggle);
        lastToggle.Populate(Utils.LocalizeTerm("Full Screen"), SetFullScreen, toggleGroup);
        toggleGroup.RegisterToggle(lastToggle.toggle);
    }

    public override void DisableWidget()
    {
        base.DisableWidget();

        SettingsController.OnScreenSizeChanged -= SettingsController_OnScreenSizeChange;

        for (int x = 0; x < ResolutionToggles.Count; ++x)
            pool.PoolObject(ResolutionToggles[x].gameObject);
        ResolutionToggles.Clear();
    }

    public void SettingsController_OnScreenSizeChange(int width, int height)
    {
        m_manualChanging = false;
        m_actualResToggle = SettingsController.currentResIndex == -1 ? ResolutionToggles.Count - 1 : SettingsController.currentResIndex;
        ResolutionToggles[m_actualResToggle].toggle.isOn = true;
        m_manualChanging = true;
    }

    private void ChangeResolution(bool isOn, int iResIndex)
    {
        if (!isOn || !m_manualChanging)
            return;

        if (iResIndex != SettingsController.currentResIndex)
            SettingsController.Instance.SetResolution(iResIndex);

        HidePopup();
    }

    private void SetFullScreen(bool isOn)
    {
        if (isOn == false || !m_manualChanging)
            return;

        if (SettingsController.currentResIndex != -1)
            SettingsController.Instance.SetFullScreen();

        HidePopup();
    }
}
