using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using GT.Store;

public class StoreBarView : MonoBehaviour
{
    public Text NameText;
    public Text IconText;
    public GTToggle SelectToggle;

    private UnityAction action;
    private Enums.StoreType StoreType;

    public void Populate(ToggleGroup group, Store store, UnityAction action)
    {
        this.action = action;
        StoreType = store.storeType;
        SelectToggle.isOn = false;
        SelectToggle.group = group;
        group.RegisterToggle(SelectToggle);
        NameText.text = Utils.LocalizeTerm(Utils.AddSpaceAfterUppercase(store.storeType.ToString()));
        IconText.text = store.icon.ToString();
        IconText.gameObject.SetActive(!string.IsNullOrEmpty(store.icon.ToString()));
    }

    public bool CheckStoreTypeChange(Enums.StoreType type)
    {
        SelectToggle.isOn = StoreType == type;
        return SelectToggle.isOn;
    }

    public void DisableToggle()
    {
        SelectToggle.group = null;
        action = null;
        SelectToggle.isOn = true;
    }

    public void OnToggleChanged(bool isOn)
    {
        if (isOn && action != null)
            action();
    }
}
