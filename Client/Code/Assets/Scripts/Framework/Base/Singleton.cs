using System;

namespace AosBaseFramework
{
    public class Singleton<T>
    {
        private static readonly T ms_instance = Activator.CreateInstance<T>();
        public static T Instance { get { return ms_instance; } }

        protected Singleton() { }
    }
}
