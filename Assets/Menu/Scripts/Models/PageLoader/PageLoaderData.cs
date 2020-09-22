using UnityEngine;
using UnityEngine.Events;

public class PageLoaderData : DynamicElement, ICustomPrefab
{
    private UnityAction action;

    private GameObject m_prefab;
    public GameObject Prefab { get { return m_prefab; } }

    public PageLoaderData(GameObject prefab, UnityAction action)
    {
        this.m_prefab = prefab;
        this.action = action;
    }

    protected override void Populate(RectTransform activeObject)
    {
        activeObject.GetComponent<PageLoaderItemView>().SetAction(action);
    }
}
