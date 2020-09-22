using UnityEngine;
using UnityEngine.Events;

public class PageLoaderItemView : MonoBehaviour
{
    public ComponentLoadingView loading;

    public void SetAction(UnityAction action)
    {
        loading.ShowLoading(transform as RectTransform);
        action();
    }
}
