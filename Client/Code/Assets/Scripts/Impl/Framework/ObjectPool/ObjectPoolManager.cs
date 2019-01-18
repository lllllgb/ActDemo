using System.Collections;
using System.Collections.Generic;
using System;

namespace AosHotfixFramework
{
    internal sealed class ObjectPoolManager : GameModuleBase, IObjectPoolManager
    {
        private readonly Dictionary<Type, ObjectPoolBase> mObjectPools;

        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        internal override void Update(float deltaTime) { }

        internal override void LateUpdate(float deltaTime) { }

        internal override void Shutdown() { }

        public ObjectPoolManager()
        {
            mObjectPools = new Dictionary<Type, ObjectPoolBase>();
        }

        public IObjectPool<T> GetObjectPool<T>() where T : IPoolObject
        {
            ObjectPoolBase tmpObjectPool;

            if (!mObjectPools.TryGetValue(typeof(T), out tmpObjectPool))
            {
                tmpObjectPool = new ObjectPool<T>();
                mObjectPools.Add(typeof(T), tmpObjectPool);
            }

            return (IObjectPool<T>)tmpObjectPool;
        }

        public ObjectPoolBase GetObjectPoolByType(Type type)
        {
            return mObjectPools[type];
        }

        public bool DestroyObjectPool<T>() where T : IPoolObject
        {
            return mObjectPools.Remove(typeof(T));
        }

        public void Release()
        {
            Dictionary<Type, ObjectPoolBase>.Enumerator tmpItor = mObjectPools.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                ((ObjectPoolBase)tmpItor.Current.Value).Release();
            }

            mObjectPools.Clear();
        }
    }
}
