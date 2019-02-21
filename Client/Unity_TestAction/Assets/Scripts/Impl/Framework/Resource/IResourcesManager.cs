using System.Threading.Tasks;

namespace AosHotfixFramework
{
    public interface IResourcesManager
    {
        void LoadManifest();

        K GetAssetByType<K>(EABType abType, string bundleName) where K : class;

        K GetAssetByType<K>(EABType abType, string bundleName, string assetName) where K : class;

        void UnLoadBundleByType(EABType type, string abName);

        void LoadBundleByType(EABType type, string abName);

        Task LoadBundleByTypeAsync(EABType abType, string abName);

        ResourcesPool ResPool { get; }
    }
}
