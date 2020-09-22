using UnityEngine;

public interface ICustomPrefab : IDynamicElement
{
    GameObject Prefab
    {
        get;
    }
}
