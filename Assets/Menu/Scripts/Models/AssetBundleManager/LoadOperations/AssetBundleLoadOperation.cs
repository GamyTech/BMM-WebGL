using System.Collections;

namespace AssetBundles
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public object Current
        {
            get
            {
                return null;
            }
        }

        public virtual float Progress
        {
            get
            {
                return 1f;
            }
        }

        public virtual int Frame
        {
            get
            {
                return 0;
            }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        abstract public bool Update();

        abstract public bool IsDone();
    }
}
