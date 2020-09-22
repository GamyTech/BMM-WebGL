using UnityEngine;

namespace AssetBundles
{
    public class AssetBundleLoadLevelOperationBase : AssetBundleLoadOperation
    {
        protected string m_AssetBundleName;
        protected string m_LevelName;
        protected bool m_IsAdditive;
        protected AsyncOperation m_Operation;
        public AsyncOperation Operation
        {
            get
            {
                return m_Operation;
            }
        }

        public AssetBundleLoadLevelOperationBase(string assetbundleName, string levelName, bool isAdditive)
        {
            m_AssetBundleName = assetbundleName;
            m_LevelName = levelName;
            m_IsAdditive = isAdditive;
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return m_Operation != null && m_Operation.isDone;
        }
    }
}
