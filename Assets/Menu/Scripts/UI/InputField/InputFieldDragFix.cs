using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class InputFieldDragFix : UIBehaviour
{
    private InputField m_inputField;
    private Button m_button;

    protected InputField inputField
    {
        get
        {
            if (m_inputField == null)
                m_inputField = GetComponentInChildren<InputField>();
            return m_inputField;
        }
    }

    protected Button button
    {
        get
        {
            if (m_button == null)
                m_button = GetComponent<Button>();
            return m_button;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        button.onClick.AddListener(OnSelect);
    }
    protected override void OnDisable()
    {
        button.onClick.RemoveListener(OnSelect);
        base.OnDisable();
    }

    public void OnSelect()
    {
        inputField.Select();
    }
}
