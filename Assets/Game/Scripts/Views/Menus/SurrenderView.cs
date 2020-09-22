using UnityEngine;
using System.Collections;

public class SurrenderView : MonoBehaviour {


    public void SurrenderButton()
    {
        RemoteGameController.Instance.Surrender();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
