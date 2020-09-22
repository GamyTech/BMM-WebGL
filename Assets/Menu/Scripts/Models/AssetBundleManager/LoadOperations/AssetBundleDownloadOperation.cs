
namespace AssetBundles
{
    public abstract class AssetBundleDownloadOperation : AssetBundleLoadOperation
    {
        bool done;

        public string assetBundleName { get; private set; }
        public LoadedAssetBundle assetBundle { get; protected set; }
        public string error { get; protected set; }
        private int m_frame = 0;
        public override int Frame
        {
            get
            {
                return m_frame;
            }
        }

        protected abstract bool downloadIsDone { get; }
        protected abstract void FinishDownload();

        public override bool Update()
        {
            if (!done && downloadIsDone)
            {
                FinishDownload();
                done = true;
            }
            ++m_frame;

            return !done;
        }

        public override bool IsDone()
        {
            return done;
        }

        public virtual void Reload()
        {
            error = null;
            done = false;
        }

        public abstract string GetSourceURL();

        public AssetBundleDownloadOperation(string assetBundleName)
        {
            this.assetBundleName = assetBundleName;
        }
    }
}
