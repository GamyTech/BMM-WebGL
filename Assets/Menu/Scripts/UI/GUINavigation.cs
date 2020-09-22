using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class GUINavigation : MonoBehaviour
{
    private GameObject CurrentSlectedObject = null;
    private Selectable CurrentSelected;
    private Selectable PreviousSelectable;

    void Update ()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {

            if (CurrentSlectedObject != EventSystem.current.currentSelectedGameObject)
            {
                CurrentSlectedObject = EventSystem.current.currentSelectedGameObject;
                CurrentSelected = CurrentSlectedObject.GetComponent<Selectable>();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    NavigateToPreviousSelectable();
                else
                    NavigateToNextSelectable();
            }

            else if (Input.GetKeyDown(KeyCode.Return))
                ValidateSelectable(); 
        }
    }

    void NavigateToNextSelectable()
    {
        PreviousSelectable = CurrentSelected;
        if (CurrentSelected == null)
            return;

        CurrentSelected = CurrentSelected.navigation.selectOnRight ?? CurrentSelected.navigation.selectOnDown;
        if (CurrentSelected == null)
            return;

        while (!CurrentSelected.IsInteractable() || !CurrentSelected.gameObject.activeInHierarchy)
        {
            CurrentSelected = CurrentSelected.navigation.selectOnRight ?? CurrentSelected.navigation.selectOnDown;
            if (CurrentSelected == null)
                return;
        }
        CurrentSelected.Select();
    }

    void NavigateToPreviousSelectable()
    {
        PreviousSelectable = CurrentSelected;

        CurrentSelected = CurrentSelected.navigation.selectOnUp;
        while (!CurrentSelected.IsInteractable() || !CurrentSelected.gameObject.activeInHierarchy)
        {
            CurrentSelected = CurrentSelected.navigation.selectOnUp;

            if (CurrentSelected == null)
                return;
        }
        CurrentSelected.Select();
    }

    void ValidateSelectable()
    {
        if(CurrentSelected is Button)
        {
            (CurrentSelected as Button).onClick.Invoke();
            return;
        }

        NavigateToNextSelectable();

        if (CurrentSelected == null)
            return;

        if (PreviousSelectable is InputField && CurrentSelected is Button)
            (CurrentSelected as Button).onClick.Invoke();
        if(CurrentSelected is Toggle)
        {
            (CurrentSelected as Toggle).isOn = true;
            NavigateToNextSelectable();
        }

        CurrentSelected.Select();
    }
}
