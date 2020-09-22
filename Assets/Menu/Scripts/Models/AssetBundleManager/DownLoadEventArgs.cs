using System;
using System.Collections.Generic;

namespace AssetBundles
{
    public class DownLoadEventArgs : EventArgs
    {
        private int m_finished;
        public int Finished
        {
            get
            {
                return m_finished;
            }
        }

        private int m_pending;
        public int Pending
        {
            get
            {
                return m_pending;
            }
        }

        private float m_averagePercent;
        public float AveragePercent
        {
            get
            {
                return m_averagePercent;
            }
        }

        private float m_actualProgress;
        public float ActualProgress
        {
            get
            {
                return m_actualProgress;
            }
        }

        public int Total
        {
            get
            {
                return Finished + Pending;
            }
        }

        private Dictionary<string, LoadedAssetBundle> m_loadedAssetBundles;
        public Dictionary<string, LoadedAssetBundle> LoadedAssetBundles
        {
            get
            {
                return m_loadedAssetBundles;
            }
        }

        public DownLoadEventArgs(Dictionary<string, LoadedAssetBundle> loadedBundles, List<AssetBundleLoadOperation> inProgressOperations, int NextOperationsCount)
        {
            m_loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
            foreach (var item in loadedBundles)
            {
                if(Utility.SUPPORTED_PLATFORMS.Contains(item.Key) == false)
                {
                    m_loadedAssetBundles.Add(item.Key, item.Value);
                }
            }
            m_finished = m_loadedAssetBundles.Count;
            m_pending = 0;
            m_averagePercent += m_finished;
            for (int i = 0; i < inProgressOperations.Count; i++)
            {
                AssetBundleDownloadOperation op = inProgressOperations[i] as AssetBundleDownloadOperation;
                if(op != null && Utility.SUPPORTED_PLATFORMS.Contains(op.assetBundleName) == false)
                {
                    m_averagePercent += inProgressOperations[i].Progress;
                    m_pending++;
                    m_actualProgress = inProgressOperations[i].Progress;
                }

            }
            m_averagePercent /= m_pending + m_finished + NextOperationsCount;
        }

        public override string ToString()
        {
            return "DownLoadEventArgs finished: " + m_finished + "\nDownloaded Bundles: " + m_loadedAssetBundles.Keys.Display() + "\nParts left: " + m_pending + "\nAverage Percent: " + m_averagePercent.ToString("p");
        }
    }
}
