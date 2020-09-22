using UnityEngine;

namespace AssetBundles
{
    public class AssetBundleLoadAssetsOperationSimulation : AssetBundleLoadAssetsOperation
    {
        Object[] m_SimulatedObjects;

        public AssetBundleLoadAssetsOperationSimulation(Object[] simulatedObjects)
        {
            m_SimulatedObjects = simulatedObjects;
        }

        public override T[] GetAssets<T>()
        {
            return System.Array.ConvertAll(m_SimulatedObjects, new System.Converter<Object, T>(o => { return o as T; }));
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}
