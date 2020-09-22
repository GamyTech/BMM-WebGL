using System;
using UnityEngine;

namespace AssetBundles
{
    public class LoadSceneEventArgs : EventArgs
    {
        private AsyncOperation m_loadingOperation;
        public AsyncOperation LoadingOperation
        {
            get
            {
                return m_loadingOperation;
            }
        }

        public LoadSceneEventArgs(AssetBundleLoadOperation operation)
        {
            AssetBundleLoadLevelOperation op = operation as AssetBundleLoadLevelOperation;
            if (op != null)
            {
                m_loadingOperation = op.Operation;
            }
        }

        public override string ToString()
        {
            return "LoadSceneEventArgs progress: " + m_loadingOperation.progress;
        }
    }
}
