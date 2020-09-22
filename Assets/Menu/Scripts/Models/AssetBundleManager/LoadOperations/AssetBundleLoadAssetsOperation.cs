
namespace AssetBundles
{
    public abstract class AssetBundleLoadAssetsOperation : AssetBundleLoadOperation
    {
        public abstract T[] GetAssets<T>() where T : UnityEngine.Object;
    }
}
