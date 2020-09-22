using UnityEngine;
using System.Collections;

public class PooledObjectsContainer : MonoBehaviour
{
    private static PooledObjectsContainer m_instance;
    public static PooledObjectsContainer Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<PooledObjectsContainer>()); }
        private set { m_instance = value; }
    }

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }
}
