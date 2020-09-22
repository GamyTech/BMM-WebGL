using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalView : MonoBehaviour
{

    public Image Medal;
    public List<GameObject> Stars;
    public Text Name;

    public void Init(Texture2D medal, int stars, string name)
    {
        Medal.sprite = medal.ToSprite();

        if (stars == 0)
            Medal.color = new Color(1f, 1f, 1f, 0.5f);

        for (int x = 0; x < stars; ++x)
            Stars[x].SetActive(true);
        for (int x = stars; x < Stars.Count; ++x)
            Stars[x].SetActive(false);


        if (string.IsNullOrEmpty(name) && stars == 0)
            Name.gameObject.SetActive(false);
        else
            Name.text = name;

    }

}
