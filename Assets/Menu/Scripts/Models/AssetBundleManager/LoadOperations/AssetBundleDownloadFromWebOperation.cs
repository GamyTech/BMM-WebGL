using UnityEngine;
using System;

namespace AssetBundles
{
    public class AssetBundleDownloadFromWebOperation : AssetBundleDownloadOperation
    {
        Func<WWW> m_GetWWW;
        WWW m_WWW;
        string m_Url;

        public override float Progress
        {
            get
            {
                return m_WWW.progress;
            }
        }


        public AssetBundleDownloadFromWebOperation(string assetBundleName, Func<WWW> getWWW)
            : base(assetBundleName)
        {
            if (getWWW == null)
                throw new System.ArgumentNullException("www");

            m_GetWWW = getWWW;
            this.m_WWW = m_GetWWW();
            m_Url = this.m_WWW.url;
        }

        protected override bool downloadIsDone { get { return (m_WWW == null) || m_WWW.isDone; } }

        protected override void FinishDownload()
        {
            error = m_WWW.error;
            if (!string.IsNullOrEmpty(error))
                return;

            AssetBundle bundle = m_WWW.assetBundle;
            if (bundle == null)
                error = string.Format("{0} is not a valid asset bundle.", assetBundleName);
            else
                assetBundle = new LoadedAssetBundle(m_WWW.assetBundle);

            m_WWW.Dispose();
            m_WWW = null;

        }

        public override void Reload()
        {
            if(m_WWW != null)
                this.m_WWW.Dispose();
            this.m_WWW = m_GetWWW();
            base.Reload();
        }

        public override string GetSourceURL()
        {
            return m_Url;
        }
    }
}
