using UnityEngine;

public class ComponentLoadingView : MonoBehaviour
{
    public GameObject LoadingPrefab;

    private SmallLoadingView loadingView;
    [SerializeField]
    private bool isInitialized = false;

    public void Initialize()
    {
        GameObject go = Instantiate(LoadingPrefab) as GameObject;
        go.InitGameObjectAfterInstantiation(transform);
        loadingView = go.GetComponent<SmallLoadingView>();
        isInitialized = true;

        loadingView.HideLoading(null, true);
    }

    public void ShowLoading(RectTransform overTransform)
    {
        if (isInitialized == false)
            Initialize();

        loadingView.ShowLoading(overTransform, null);
    }

    public void HideLoading(bool instant = false)
    {
        if (isInitialized == false)
            Initialize();
        else
            loadingView.HideLoading(null, instant);
    }

}
