namespace AosHotfixFramework
{
	public static class EntityFactory
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

        public static T Create<T>() where T : Entity
		{
			T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer);
            }

            return tmpDisposer;
        }

		public static T Create<T, A>(A a) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a);
            }

            return tmpDisposer;
        }

        public static T Create<T, A, B>(A a, B b) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b);
            }

            return tmpDisposer;
        }

		public static T Create<T, A, B, C>(A a, B b, C c) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b, c);
            }

            return tmpDisposer;
        }

		public static T CreateWithId<T>(long id) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Id = id;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer);
            }

            return tmpDisposer;
        }

		public static T CreateWithId<T, A>(long id, A a) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Id = id;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a);
            }

            return tmpDisposer;
        }

		public static T CreateWithId<T, A, B>(long id, A a, B b) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Id = id;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b);
            }

            return tmpDisposer;
        }

		public static T CreateWithId<T, A, B, C>(long id, A a, B b, C c) where T : Entity
        {
            T tmpDisposer = GetObjectPool<T>().Spawn();
            tmpDisposer.IsFromPool = true;
            tmpDisposer.Id = id;

            var tmpObjMgr = GameModuleManager.GetModule<IObjectManager>();
            if (null != tmpObjMgr)
            {
                tmpObjMgr.Awake(tmpDisposer, a, b, c);
            }

            return tmpDisposer;
        }
	}
}
