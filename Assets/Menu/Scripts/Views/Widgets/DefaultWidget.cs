using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefaultWidget : Widget {

    //Testing
    private Text m_testingText;
    protected Text testingText
    {
        get
        {
            if (m_testingText == null)
                m_testingText = GetComponent<Text>();
            return m_testingText;
        }
        set { m_testingText = value; }
    }


    public override void EnableWidget()
    {
        base.EnableWidget();

        //testing
        ShowName();
    }

    #region Testing
    private void ShowName()
    {
        if (testingText == null)
            testingText = GetComponent<Text>();

        if (testingText != null)
            testingText.text = ToString();
    }
    #endregion Testing
}
