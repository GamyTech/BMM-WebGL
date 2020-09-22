using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    static Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        foreach (var pool in pools)
        {
            foreach (var go in pool.Value)
                Destroy(go);
            pool.Value.Clear();
        }
        pools.Clear();
    }

    public static GameObject GetObject(GameObject ofType)
    {
        Stack<GameObject> pooledObjects;
        GameObject go;
        if (pools.TryGetValue(ofType.name, out pooledObjects) && pooledObjects.Count > 0)
            go = pooledObjects.Pop();
        else
        {
            go = Instantiate(ofType);
            go.name = ofType.name;
        }
        go.SetActive(true);
        return go;
    }

    public static void PoolObject(GameObject objectToPool)
    {
        if (PooledObjectsContainer.Instance == null)
        {
            Destroy(objectToPool);
            return;
        }

        Stack<GameObject> pooledObjects;
        objectToPool.SetActive(false);
        objectToPool.transform.SetParent(PooledObjectsContainer.Instance.transform);

        if (pools.TryGetValue(objectToPool.name, out pooledObjects))
            pooledObjects.Push(objectToPool);
        else
        {
            pooledObjects = new Stack<GameObject>();
            pooledObjects.Push(objectToPool);
            pools.Add(objectToPool.name, pooledObjects);
        }
    }
}
