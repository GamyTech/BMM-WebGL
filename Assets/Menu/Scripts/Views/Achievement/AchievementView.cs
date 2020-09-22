using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementView : MonoBehaviour
{
    private Achievement achievement;
    public Text m_description;
    public Slider m_slider;
    public Text m_progess;
    public Button ClaimButton;
    public Image OpenGift;
    public Image CloseGift;
    public Image ClaimPanel;
    [HideInInspector]
    public bool Completed;
    [HideInInspector]
    public int ID;

    public void Populate(Achievement achievement)
    {
        this.achievement = achievement;
        Init();
    }

    public void Init()
    {
        ID = achievement.ID;
        m_description.text = Utils.LocalizeTerm(achievement.description);
        m_slider.value = (float)achievement.progess / (float)achievement.step;
        m_progess.text = achievement.progess + "/" + achievement.step;

        if (achievement.collected)
        {
            Completed = true;
            CloseGift.color = Color.white;
            OpenGift.color = new Color(1.0f,1.0f,1.0f, 0.5f);
            ClaimButton.interactable = false;
            OpenGift.gameObject.SetActive(true);
            CloseGift.gameObject.SetActive(false);
            ClaimPanel.gameObject.SetActive(false);
        }   

        else
        {
            OpenGift.gameObject.SetActive(false);
            CloseGift.gameObject.SetActive(true);
            OpenGift.color = Color.white;

            Completed = achievement.step == achievement.progess;
            if (Completed)
            {
                CloseGift.color = Color.white;
                ClaimButton.interactable = true;
                ClaimPanel.gameObject.SetActive(true);
            }

            else
            {
                CloseGift.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                ClaimButton.interactable = false;
                ClaimPanel.gameObject.SetActive(false);
            }
        }
    }

    public void Claim()
    {

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
