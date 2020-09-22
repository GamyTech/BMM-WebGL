using UnityEngine;

public class CameraAdjust : MonoBehaviour
{

    public static CameraAdjust Instance { get; private set; }

    public float ratioMin = 1.286f;
    public float sizeAtMin = 3.75f;
    public float WitdhAdjust = 5f;
    public float HeightAdjust = 1f;

    private float startingOrthoSize;
    public Camera OrthoCam;

    void Start()
    {
        MainSceneController.Instance.MainCamera.tag = "Untagged";

        Instance = this;
        startingOrthoSize = OrthoCam.orthographicSize;
        CameraAjustment(Screen.currentResolution.width, Screen.currentResolution.height);
        SettingsController.OnScreenSizeChanged += CameraAjustment;
    }

    void OnDestroy()
    {
        MainSceneController.Instance.MainCamera.tag = "MainCamera";

        Instance = null;
        OrthoCam.orthographicSize = startingOrthoSize;
        SettingsController.OnScreenSizeChanged -= CameraAjustment;
    }

    private void CameraAjustment(int width, int height)
    {
        if (OrthoCam.aspect > ratioMin)
            OrthoCam.orthographicSize = sizeAtMin;
        else
            OrthoCam.orthographicSize = (WitdhAdjust / OrthoCam.aspect) + HeightAdjust / OrthoCam.aspect;
    }
}
