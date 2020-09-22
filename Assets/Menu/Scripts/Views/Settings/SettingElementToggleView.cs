using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[ExecuteInEditMode]
public class SettingElementToggleView : MonoBehaviour {

    public string _text = null;
    public Sprite _sprite = null;
    public Enums.ActionType _action;
    public Enums.PlayerPrefsVariable _actionString;


    private Toggle m_toggle;
    public Toggle toggle
    {
        get
        {
            if (m_toggle == null)
            {
                m_toggle = GetComponentInChildren<Toggle>();
            }
            return m_toggle;
        }
    }

    void OnEnable()
    {
        //assigning the action tor the toggle button if exist, and sets the default value 
        if (_action == Enums.ActionType.ChangePref)
        {
            string currentPref = GTDataManagementKit.GetFromPrefs(_actionString);
            toggle.isOn = currentPref.ParseBool();
        }
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(ActionKit.CreateActionBool(_action, _actionString.ToString()));
    }

    void OnDisable()
    {
        m_toggle = null;
    }
}
