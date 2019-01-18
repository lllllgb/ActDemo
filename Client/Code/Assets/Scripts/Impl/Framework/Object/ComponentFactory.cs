using System;

namespace AosHotfixFramework
{
	public static class ComponentFactory
	{
        static IObjectPool<T> GetObjectPool<T>() where T : Component
        {
            IObjectPoolManager tmpPoolMgr = GameModuleManager.GetModule<IObjectPoolManager>();
            IObjectPool<T> tmpObjPool = null;

            if (null != tmpPoolMgr)
            {
                tmpObjPool = tmpPoolMgr.GetObjectPool<T>();
            }

            return tmpObjPool;
        }

        public static T Create<T>(Entity entity) where T : Component
		{
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Parent = entity;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer);
            }

			return tmpDisposer;
		}

		public static T Create<T, A>(Entity entity, A a) where T : Component
		{
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Parent = entity;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a);
            }

            return tmpDisposer;
		}

		public static T Create<T, A, B>(Entity entity, A a, B b) where T : Component
		{
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Parent = entity;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b);
            }

            return tmpDisposer;
        }

		public static T Create<T, A, B, C>(Entity entity, A a, B b, C c) where T : Component
		{
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Parent = entity;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b, c);
            }

            return tmpDisposer;
        }
	}
}
