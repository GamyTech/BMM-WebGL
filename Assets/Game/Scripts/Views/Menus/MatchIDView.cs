using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchIDView : MonoBehaviour {

    public Text text;

    public void SetMatchID(int id)
    {
        text.text = Utils.LocalizeTerm("Game ID") +"\n"+ id;
    }

}
