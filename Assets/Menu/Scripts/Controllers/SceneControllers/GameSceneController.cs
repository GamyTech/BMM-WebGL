using UnityEngine;

public class GameSceneController : MonoBehaviour
{

    public static void InitializeScene()
    {
        Debug.Log("InitializeScene Game");
        LoadingController.Instance.HidePageLoading();
    }
}
