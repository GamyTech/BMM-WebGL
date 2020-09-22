using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// The pooled objects currently available.
    /// </summary>
    private Stack<GameObject> pooledObjects = new Stack<GameObject>();

    [SerializeField]
    private int poolSize = 10;
    [SerializeField]
    private bool keepActive = false;
    public GameObject objectPrefab;

    void OnDestroy()
    {
        foreach (var item in pooledObjects)
            Destroy(item);
        pooledObjects.Clear();
    }

    public void PoolObject(GameObject go)
    {
        try
        {
            if (PooledObjectsContainer.Instance != null && pooledObjects.Count < poolSize)
            {
                go.transform.SetParent(PooledObjectsContainer.Instance.transform, false);
                if (!keepActive)
                    go.SetActive(false);
                pooledObjects.Push(go);
            }
            else
                Destroy(go);
        }
        catch (Exception e)
        {
            Debug.Log("Exception " + e.Message);
            Destroy(go);
        }
    }

    public void PoolObjects(IEnumerable<GameObject> objs)
    {
        foreach (var item in objs)
            PoolObject(item);
    }

    public GameObject GetObjectFromPool()
    {
        if (pooledObjects.Count == 0)
            return Instantiate(objectPrefab);

        GameObject go = pooledObjects.Pop();
        if (keepActive == false)
            go.SetActive(true);
        return go;
    }

    private void InstantiateBuffer()
    {
        if (objectPrefab == null)
        {
            Debug.LogError("ObjectPool Prefab is null");
            return;
        }

        for (int i = 0; i < poolSize; i++)
            PoolObject(Instantiate(objectPrefab));
    }
}
