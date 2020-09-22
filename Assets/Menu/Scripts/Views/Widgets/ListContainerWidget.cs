using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.User;

public abstract class ListContainerWidget<T> : PopupWidget where T : FragmentedList<FragmentedListDynamicElement>
{

    public HorizontalOrVertcalDynamicContentLayoutGroup content;

    public GameObject PageLoadingPrefab;
    private IDynamicElement pageLoaderElement;
    private int loadedHistory = 0;

    public override void DisableWidget()
    {
        content.ClearElements();
    }

    protected void InitList(T fragmentedList)
    {
        content.SetElements(new List<IDynamicElement>(fragmentedList.ElementsList.ToArray()));
        loadedHistory = fragmentedList.ElementsList.Count;

        if (fragmentedList.GetNextUnupdatedIndex() < fragmentedList.ElementsList.Count)
            UpdateNextNeededElement();
        else if (!fragmentedList.IsListComplete)
        {
            pageLoaderElement = new PageLoaderData(PageLoadingPrefab, UpdateNextNeededElement);
            content.AddElement(pageLoaderElement);
        }
    }

    protected void UpdateList(T newValue)
    {
        if (newValue != null && newValue.ElementsList.Count != loadedHistory)
            InitList(newValue);
    }

    protected abstract void UpdateNextNeededElement();
}
