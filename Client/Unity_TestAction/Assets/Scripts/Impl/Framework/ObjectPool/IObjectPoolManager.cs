
using System;

namespace AosHotfixFramework
{
    public interface IObjectPoolManager
    {
        IObjectPool<T> GetObjectPool<T>() where T : IPoolObject;

        ObjectPoolBase GetObjectPoolByType(Type type);

        bool DestroyObjectPool<T>() where T : IPoolObject;

        void Release();
    }
}
