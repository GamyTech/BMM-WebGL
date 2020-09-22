using UnityEngine;

namespace AssetBundles
{
    public class AssetBundleLoadAssetsOperationFull : AssetBundleLoadAssetsOperation
    {
        protected string m_AssetBundleName;
        protected string m_DownloadingError;
        protected System.Type m_Type;
        protected AssetBundleRequest m_Request = null;

        public AssetBundleLoadAssetsOperationFull(string bundleName, System.Type type)
        {
            m_AssetBundleName = bundleName;
            m_Type = type;
        }

        public override T[] GetAssets<T>()
        {
            if (m_Request != null && m_Request.isDone)
                return System.Array.ConvertAll(m_Request.allAssets, new System.Converter<Object, T>(o => { return o as T; }));
            else
                return null;
        }

        // Returns true if more Update calls are required.
        public override bool Update()
        {
            if (m_Request != null)
                return false;

            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                ///@TODO: When asset bundle download fails this throws an exception...
                m_Request = bundle.m_AssetBundle.LoadAllAssetsAsync(m_Type);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && m_DownloadingError != null)
            {
                Debug.LogError(m_DownloadingError);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }
    }
}
