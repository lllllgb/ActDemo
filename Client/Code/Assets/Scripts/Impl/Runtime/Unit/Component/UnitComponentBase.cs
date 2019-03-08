using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using System;

namespace AosHotfixRunTime
{
    public abstract class UnitComponentBase : IReference
    {
        public interface IInit0
        {
            void Init();
        }

        public interface IInit1<A>
        {
            void Init(A a);
        }

        public interface IInit2<A, B>
        {
            void Init(A a, B b);
        }

        private Unit mParent;
        public Unit Parent { get { return mParent; } }

        public bool IsDispose { get; private set; }

        public virtual void OnInit()
        {
            IsDispose = false;
        }

        public virtual void Dispose()
        {
            mParent = null;
            IsDispose = true;
        }

        public void SetParent(Unit parent)
        {
            mParent = parent;
        }

        public virtual void Update(float deltaTime)
        {
        }
    }

    public partial class Unit
    {
        private readonly List<UnitComponentBase> mComponentList = new List<UnitComponentBase>();
        private readonly Dictionary<Type, UnitComponentBase> mComponentDict = new Dictionary<Type, UnitComponentBase>();

        public K AddComponent<K>() where K : UnitComponentBase, new()
        {
            K tmpComponent = ReferencePool.Fetch<K>();

            if (this.mComponentDict.ContainsKey(tmpComponent.GetType()))
            {
                throw new Exception($"AddComponent, component already exist, component: {typeof(K).Name}");
            }

            tmpComponent.SetParent(this);
            mComponentList.Add(tmpComponent);
            mComponentDict.Add(tmpComponent.GetType(), tmpComponent);

            if (tmpComponent is UnitComponentBase.IInit0)
            {
                (tmpComponent as UnitComponentBase.IInit0).Init();
            }

            return tmpComponent;
        }

        public K AddComponent1<K, A>(A a) where K : UnitComponentBase, new()
        {
            K tmpComponent = AddComponent<K>();

            if (tmpComponent is UnitComponentBase.IInit1<A>)
            {
                (tmpComponent as UnitComponentBase.IInit1<A>).Init(a);
            }

            return tmpComponent;
        }

        public K AddComponent2<K, A, B>(A a, B b) where K : UnitComponentBase, new()
        {
            K tmpComponent = AddComponent<K>();

            if (tmpComponent is UnitComponentBase.IInit2<A, B>)
            {
                (tmpComponent as UnitComponentBase.IInit2<A, B>).Init(a, b);
            }

            return tmpComponent;
        }

        public K GetComponent<K>() where K : UnitComponentBase
        {
            UnitComponentBase tmpComponent;

            if (!this.mComponentDict.TryGetValue(typeof(K), out tmpComponent))
            {
                return default(K);
            }

            return (K)tmpComponent;
        }

        private void UpdateComponents(float deltaTime)
        {
            for (int i = mComponentList.Count - 1; i >= 0; --i)
            {
                var tmpComponent = mComponentList[i];

                if (tmpComponent.IsDispose)
                {
                    ReferencePool.Recycle(tmpComponent);
                    mComponentList.RemoveAt(i);
                }
                else
                {
                    tmpComponent.Update(deltaTime);
                }
            }
        }

        public void RemoveComponent<K>() where K : UnitComponentBase
        {
            UnitComponentBase tmpComponent;

            if (!this.mComponentDict.TryGetValue(typeof(K), out tmpComponent))
            {
                return;
            }

            mComponentDict.Remove(typeof(K));
            tmpComponent.Dispose();
        }

        private void DisposeComponents()
        {
            for (int i = mComponentList.Count - 1; i >= 0; --i)
            {
                var tmpComponent = mComponentList[i];
                tmpComponent.Dispose();
                ReferencePool.Recycle(tmpComponent);
            }

            mComponentList.Clear();
            mComponentDict.Clear();
        }
    }
}
