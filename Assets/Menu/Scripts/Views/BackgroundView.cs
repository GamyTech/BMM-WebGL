using UnityEngine;
using UnityEngine.UI;

public class BackgroundView : MonoBehaviour
{
    public Image backgroundImage;
    public Image PCBackgroundImage;

    void Start()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        backgroundImage.gameObject.SetActive(false);
        PCBackgroundImage.gameObject.SetActive(true);
        PCBackgroundImage.sprite = AssetController.Instance.GetGameSpecific().PCBackground;
#else
        backgroundImage.gameObject.SetActive(true);
        PCBackgroundImage.gameObject.SetActive(false);
        backgroundImage.sprite = AssetController.Instance.GetGameSpecific().background;
#endif
    }
}
