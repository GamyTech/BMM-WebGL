using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundles
{
    public class AssetBundleLoadLevelOperation : AssetBundleLoadLevelOperationBase
    {
        protected string m_DownloadingError;

        public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive) :
            base(assetbundleName, levelName, isAdditive) { }

        public override bool Update()
        {
            if (m_Operation != null)
                return false;

            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                m_Operation = SceneManager.LoadSceneAsync(m_LevelName, m_IsAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                return false;
            }
            else
                return true;
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Operation == null && m_DownloadingError != null)
            {
                Debug.LogError(m_DownloadingError);
                return true;
            }

            return base.IsDone();
        }
    }
}
