using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    public Camera cam;
    public Canvas canvas;

    public Vector3 ReferenceScale = new Vector3(.01f, .01f, .01f);
    public Vector2 ReferenceResolution = new Vector2(1334, 750);
    public Vector3 NormalCameraRotation = new Vector3(0, 0, 0);

    private const float MAX_RATIO = 1.78f;
    private const float MIN_RATIO = 1.33333f;

    void OnEnable()
    {
        SettingsController.OnScreenSizeChanged += SettingsController_OnScreenSizeChanged;
    }

    void OnDisable()
    {
        SettingsController.OnScreenSizeChanged -= SettingsController_OnScreenSizeChanged;
    }

    void Awake()
    {
        SetChanged();
    }

    private void SetChanged()
    {
        Utils.ChangeOrientation(this);

        Quaternion q = cam.transform.rotation;
        q.eulerAngles = NormalCameraRotation;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        cam.transform.rotation = q;
    }

    private void CanvasAdjust()
    {
        float refFactor = ReferenceResolution.x / ReferenceResolution.y;
        float factor = Screen.height / (float)Screen.width;
        float nFactor = refFactor / factor;

        float newHeight = nFactor * ReferenceResolution.y;
        float newWidth = ReferenceResolution.x;

        RectTransform rect = canvas.transform as RectTransform;
        rect.sizeDelta = new Vector2(newWidth, newHeight);

        rect.localScale = ReferenceScale;
    }

    private void SettingsController_OnScreenSizeChanged(int width, int height)
    {
        SetChanged();
    }
}
