
namespace AosHotfixFramework
{
    public interface IObjectPool<T> where T : IPoolObject
    {
        T Spawn();
    }
}
