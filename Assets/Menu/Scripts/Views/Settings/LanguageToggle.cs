using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LanguageToggle : MonoBehaviour
{
    private Toggle m_toggle;
    private Text m_text;

    public Toggle toggle
    {
        get
        {
            if (m_toggle == null)
                m_toggle = GetComponent<Toggle>();
            return m_toggle;
        }
    }

    public Text text
    {
        get
        {
            if (m_text == null)
                m_text = GetComponentInChildren<Text>();
            return m_text;
        }
    }

    public void Populate(string text, UnityEngine.Events.UnityAction<bool> onToggle, ToggleGroup group = null)
    {
        this.text.text = text;
        this.toggle.onValueChanged.AddListener(onToggle);
        if(group != null)
            this.toggle.group = group;
    }
}
