using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FontToggleChange : MonoBehaviour
{
    public Text textToChange;

    public Font fontOn;
    public Font fontOff;

    void OnEnable()
    {
        GTToggle toggle = GetComponent<GTToggle>();
        toggle.AddListener(OnValueChanged);

        OnValueChanged(toggle.isOn);
    }

    void OnDisable()
    {
        GTToggle toggle = GetComponent<GTToggle>();
        toggle.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
        textToChange.font = isOn ? fontOn : fontOff;
    }
}
