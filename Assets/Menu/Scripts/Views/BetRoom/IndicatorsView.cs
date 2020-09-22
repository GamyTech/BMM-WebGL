using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IndicatorsView : MonoBehaviour
{

    private ToggleGroup m_group;
    protected ToggleGroup group
    {
        get
        {
            if (m_group == null)
                m_group = GetComponent<ToggleGroup>();
            return m_group;
        }
    }

    private ObjectPool m_indicatorsPool;
    protected ObjectPool indicatorsPool
    {
        get
        {
            if (m_indicatorsPool == null)
                m_indicatorsPool = GetComponent<ObjectPool>();
            return m_indicatorsPool;
        }
    }

    private List<Toggle> Indicators = new List<Toggle>();

    public void Init(int count, int selectedIndex)
    {
        RemoveAll();
        for (int i = 0; i < count; i++)
        {
            Toggle toggle = indicatorsPool.GetObjectFromPool().GetComponent<Toggle>();
            toggle.gameObject.InitGameObjectAfterInstantiation(transform);
            toggle.group = group;
            group.RegisterToggle(toggle);
            Indicators.Add(toggle);
        }
        SelectIndex(selectedIndex);
    }

    public void SelectIndex(int index)
    {
        group.SetAllTogglesOff();
        if (Indicators.IsValidIndex(index))
            Indicators[index].isOn = true;
    }

    private void OnDestroy()
    {
        RemoveAll();
    }

    private void RemoveAll()
    {
        List<GameObject> objects = new List<GameObject>();
        for (int x = 0; x < Indicators.Count; x++)
        {
            group.UnregisterToggle(Indicators[x]);
            objects.Add(Indicators[x].gameObject);
        }

        indicatorsPool.PoolObjects(objects);
        Indicators.Clear();
    }
}
