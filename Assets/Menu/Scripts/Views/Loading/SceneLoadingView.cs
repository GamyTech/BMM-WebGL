using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using AssetBundles;

public class SceneLoadingView : MonoBehaviour
{
    private enum ProgressFormat { Simple, Debug }
    private enum DescriptionState { Shown, Hidden }

    private FadingElement m_fadingElement;
    public FadingElement fadingElement
    {
        get
        {
            if (m_fadingElement == null)
                m_fadingElement = GetComponentInChildren<FadingElement>(true);
            return m_fadingElement;
        }
    }

    public GameObject SpecificLoadingPanel;
    public GameObject TotalLoadingPanel;
    public Text TotaldescriptionText;
    public Slider totalProgressBar;
    public Text TotalPercentText;
    public Slider SpecificProgressBar;
    public Text SpecificPercentText;
    public Text SpecificdescriptionText;
    public Image backgroundImage;
    public Image PCBackgroundImage;
    public Animator logoAnim;

    private Image currentBackground;

    private int currentAssetDescriptionsIndex = 0;
    private string[] assetDescriptions = new string[]
    {
        "Connecting to the network",
        "Securing connection",
        "Downloading the game",
        "Turning on the lights",
        "Setting up the board",
        "Stacking the chips",
        "Creating the animations",
        "Rolling the dice",
        "Mixing the drinks",
        "Killing puppies",
    };

    [SerializeField]
    private float moveTime = 0.1f;
    [SerializeField]
    private ProgressFormat percentFormat;
    [SerializeField]
    private DescriptionState descriptionState;

    float currentTotalProgress = 0;
    float targetTotalProgress = 0;
    float currentSpecificProgress = 0;
    float targetSpecificProgress = 0;

    float velocity = 0;
    private bool fadeBackgroundToWhite = false;

    bool isOn = false;

    void Start()
    {
        SetLoadingInfoText(assetDescriptions[0], true);
        GetLoadingData();
    }

    void Update()
    {
        if (currentTotalProgress != targetTotalProgress)
        {
            currentTotalProgress = totalProgressBar.value = Mathf.SmoothDamp(currentTotalProgress, targetTotalProgress, ref velocity, moveTime);
            if (TotalPercentText != null)
                TotalPercentText.text = FormatProgressText(percentFormat, currentTotalProgress);
            if (fadeBackgroundToWhite)
                currentBackground.color = new Color(currentTotalProgress, currentTotalProgress, currentTotalProgress);
        }

        if (currentSpecificProgress != targetSpecificProgress)
        {
            if (currentSpecificProgress < targetSpecificProgress)
            {
                currentSpecificProgress = SpecificProgressBar.value = Mathf.SmoothDamp(currentSpecificProgress, targetSpecificProgress, ref velocity, moveTime);
                SpecificPercentText.text = FormatProgressText(percentFormat, currentSpecificProgress);
            }
            else
            {
                SetLoadingInfoText(assetDescriptions[Mathf.Clamp(++currentAssetDescriptionsIndex, 0, assetDescriptions.Length - 1)], true);
                SpecificPercentText.text = FormatProgressText(percentFormat, 0);
                currentSpecificProgress = SpecificProgressBar.value = targetSpecificProgress;
            }
        }
    }

    public void StartAssetLoading()
    {
        AssetBundleManager.OnDownLoadProgressChanged += AssetBundleManager_OnDownLoadProgressChanged;
        SetTotalProgressBar(0, true);
        HideSpecifProgressBar();
    }

    public void StopAssetLoading()
    {
        AssetBundleManager.OnDownLoadProgressChanged -= AssetBundleManager_OnDownLoadProgressChanged;
        SetTotalProgressBar(0, true);
        HideSpecifProgressBar();
    }

    public void SetLoadingInfoText(string text, bool overrideShow = false)
    {
        SpecificdescriptionText.enabled = (descriptionState == DescriptionState.Shown || overrideShow) && !string.IsNullOrEmpty(text);
        if (SpecificdescriptionText != null)
            SpecificdescriptionText.text = Utils.LocalizeTerm(text).ToUpper();
    }

    public void SetSpecificProgressBar(float progress, bool start)
    {
        targetSpecificProgress = progress;
        if (start)
        {
            SpecificProgressBar.value = 0;
        }
    }

    public void SetTotalProgressBar(float progress, bool start)
    {
        if (TotalLoadingPanel.activeSelf == false && progress!= 0)
            TotalLoadingPanel.SetActive(true);
        targetTotalProgress = progress;
        if (start)
        {
            currentTotalProgress = 0;
        }
    }


    public void HideSpecifProgressBar()
    {
        TotaldescriptionText.text = Utils.LocalizeTerm("LOADING GAME");
        SpecificLoadingPanel.SetActive(false);
    }
    public void ShowSpecifProgressBar()
    {
        TotaldescriptionText.text = Utils.LocalizeTerm("TOTAL"); ;
        SpecificLoadingPanel.SetActive(true);
    }

    public void ShowLoading(Action finishedCallback, bool instant = false)
    {
        if (isOn)
        {
            if (finishedCallback != null)
                finishedCallback();
            return;
        }
        isOn = true;
        fadingElement.FadeIn(instant, finishedCallback);
    }

    public void HideLoading(Action finishedCallback, bool instant = false)
    {
        if (isOn == false)
        {
            if (finishedCallback != null)
                finishedCallback();
            return;
        }
        fadingElement.FadeOut(instant, () =>
        {
            isOn = false;
            currentTotalProgress = 0;

            //backgroundImage.sprite = ContentController.DefaultSpritesDict[Enums.DefaultTexture.Splash];
            //backgroundImage.color = new Color(.3f, .3f, .3f);

            totalProgressBar.gameObject.SetActive(false);
            if (finishedCallback != null)
                finishedCallback();
        });
    }

    private string FormatProgressText(ProgressFormat format, float percent)
    {
        if (float.IsNaN(percent)) percent = 0;
        string str;
        switch (format)
        {
            case ProgressFormat.Simple:
                str = (percent * 100).ToString("N0") + "%";
                break;
            default:
                str = "[" + (percent * 100).ToString("N0") + "%] ";
                break;
        }
        return str;
    }

    private void AssetBundleManager_OnDownLoadProgressChanged(DownLoadEventArgs args)
    {
        SetSpecificProgressBar(args.ActualProgress, false);
        SetTotalProgressBar(args.AveragePercent, false);
    }

    private void GetLoadingData()
    {
        logoAnim.runtimeAnimatorController = AssetController.Instance.loadingData.logoAnim;

#if UNITY_STANDALONE || UNITY_WEBGL
        fadeBackgroundToWhite = AssetController.Instance.loadingData.FadeToWhite;
        PCBackgroundImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(false);
        currentBackground = PCBackgroundImage;
        currentBackground.sprite = AssetController.Instance.loadingData.PCBackground;
#else
        fadeBackgroundToWhite = false;
        backgroundImage.gameObject.SetActive(true);
        PCBackgroundImage.gameObject.SetActive(false);
        currentBackground = backgroundImage;
        currentBackground.sprite = AssetController.Instance.loadingData.Background;
#endif


        if (currentBackground.sprite == null || fadeBackgroundToWhite)
            currentBackground.color = Color.black;
        else
            currentBackground.color = Color.white;

    }
}
