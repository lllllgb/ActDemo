using System;
using System.Collections.Generic;
using AosBaseFramework;

namespace AosHotfixFramework
{
    /// <summary>
    /// 引用池。
    /// </summary>
    public static class ReferencePool
    {
        private static readonly IDictionary<Type, EQueue<IReference>> s_ReferencePool = new Dictionary<Type, EQueue<IReference>>();

        /// <summary>
        /// 清除所有引用池。
        /// </summary>
        public static void ClearAll()
        {
            lock (s_ReferencePool)
            {
                s_ReferencePool.Clear();
            }
        }

        /// <summary>
        /// 清除引用池。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        public static void Clear<T>() where T : class, IReference
        {
            lock (s_ReferencePool)
            {
                GetReferencePool(typeof(T)).Clear();
            }
        }

        /// <summary>
        /// 清除引用池。
        /// </summary>
        /// <param name="referenceType">引用类型。</param>
        public static void Clear(Type referenceType)
        {
            if (referenceType == null)
            {
                Logger.LogError("Reference type is invalid.");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                Logger.LogError("Reference type is not a non-abstract class type.");
            }

            if (!typeof(IReference).IsAssignableFrom(referenceType))
            {
                Logger.LogError(string.Format("Reference type '{0}' is invalid.", referenceType.FullName));
            }

            lock (s_ReferencePool)
            {
                GetReferencePool(referenceType).Clear();
            }
        }

        /// <summary>
        /// 获取引用池的数量。
        /// </summary>
        /// <returns>引用池的数量。</returns>
        public static int Count()
        {
            lock (s_ReferencePool)
            {
                return s_ReferencePool.Count;
            }
        }

        /// <summary>
        /// 获取引用池中引用的数量。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <returns>引用池中引用的数量。</returns>
        public static int Count<T>()
        {
            lock (s_ReferencePool)
            {
                return GetReferencePool(typeof(T)).Count;
            }
        }

        /// <summary>
        /// 获取引用池中引用的数量。
        /// </summary>
        /// <param name="referenceType">引用类型。</param>
        /// <returns>引用池中引用的数量。</returns>
        public static int Count(Type referenceType)
        {
            if (referenceType == null)
            {
                Logger.LogError("Reference type is invalid.");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                Logger.LogError("Reference type is not a non-abstract class type.");
            }

            if (!typeof(IReference).IsAssignableFrom(referenceType))
            {
                Logger.LogError(string.Format("Reference type '{0}' is invalid.", referenceType.FullName));
            }

            lock (s_ReferencePool)
            {
                return GetReferencePool(referenceType).Count;
            }
        }

        /// <summary>
        /// 从引用池获取引用。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        public static T Fetch<T>() where T : class, IReference
        {
            T tmpInstance = default(T);

            lock (s_ReferencePool)
            {
                EQueue<IReference> referencePool = GetReferencePool(typeof(T));
                if (referencePool.Count > 0)
                {
                    tmpInstance = (T)referencePool.Dequeue();
                }
            }

            if (null == tmpInstance)
                tmpInstance = Activator.CreateInstance<T>();

            tmpInstance.IsFromPool = true;

            return tmpInstance;
        }

        /// <summary>
        /// 将引用归还引用池。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <param name="reference">引用。</param>
        public static void Recycle(IReference reference)
        {
            if (reference == null)
            {
                Logger.LogError("Reference is invalid.");
            }
            
            lock (s_ReferencePool)
            {
                GetReferencePool(reference.GetType()).Enqueue(reference);
            }
        }

        private static EQueue<IReference> GetReferencePool(Type refType)
        {
            EQueue<IReference> referencePool = null;

            if (!s_ReferencePool.TryGetValue(refType, out referencePool))
            {
                referencePool = new EQueue<IReference>();
                s_ReferencePool.Add(refType, referencePool);
            }

            return referencePool;
        }
    }
}
