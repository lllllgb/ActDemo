using System;
using System.Collections.Generic;
using AosBaseFramework;

namespace AosHotfixFramework
{
	public interface IObjectEventFacade
	{
		Type Type();
		void Set(object value);
	}

	public abstract class ObjectEventFacade<T> : IObjectEventFacade
	{
		private T value;

		protected T Get()
		{
			return value;
		}

		public void Set(object v)
		{
			this.value = (T)v;
		}

		public Type Type()
		{
			return typeof(T);
		}
	}

    internal sealed class ObjectManager : GameModuleBase, IObjectManager
	{
		private readonly Dictionary<Type, IObjectEventFacade> disposerEvents = new Dictionary<Type, IObjectEventFacade>();

		private Queue<ObjectDisposer> updates = new Queue<ObjectDisposer>();
		private Queue<ObjectDisposer> updates2 = new Queue<ObjectDisposer>();

		private readonly Queue<ObjectDisposer> starts = new Queue<ObjectDisposer>();

		private Queue<ObjectDisposer> loaders = new Queue<ObjectDisposer>();
		private Queue<ObjectDisposer> loaders2 = new Queue<ObjectDisposer>();

		private Queue<ObjectDisposer> lateUpdates = new Queue<ObjectDisposer>();
		private Queue<ObjectDisposer> lateUpdates2 = new Queue<ObjectDisposer>();

		private readonly HashSet<ObjectDisposer> unique = new HashSet<ObjectDisposer>();

		public ObjectManager()
		{
			this.disposerEvents.Clear();
            
			Type[] types = HotFixHelper.GetHotfixTypes();
			foreach (Type type in types)
			{
				object[] attrs = type.GetCustomAttributes(typeof(ObjectEventFacadeAttribute), false);

				if (attrs.Length == 0)
				{
					continue;
				}

				object obj = Activator.CreateInstance(type);
				IObjectEventFacade objectSystem = obj as IObjectEventFacade;
				if (objectSystem == null)
				{
					Logger.LogError($"组件事件没有继承IObjectEvent: {type.Name}");
                    
					continue;
				}
				this.disposerEvents[objectSystem.Type()] = objectSystem;
			}

			this.Load();
		}
        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        internal override void Update(float deltaTime)
        {
            Update();
        }

        internal override void LateUpdate(float deltaTime)
        {
            LateUpdate();
        }

        internal override void Shutdown()
        {
        }

        public void Add(ObjectDisposer disposer)
		{
            IObjectEventFacade objectSystem;

            if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectSystem))
			{
				return;
			}

			if (objectSystem is ILoad)
			{
				this.loaders.Enqueue(disposer);
			}

			if (objectSystem is IUpdate)
			{
				this.updates.Enqueue(disposer);
			}

			if (objectSystem is IStart)
			{
				this.starts.Enqueue(disposer);
			}

			if (objectSystem is ILateUpdate)
			{
				this.lateUpdates.Enqueue(disposer);
			}
		}

		public void Awake(ObjectDisposer disposer)
		{
			this.Add(disposer);
            IObjectEventFacade objectEvent;

            if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
			{
				return;
			}
			IAwake iAwake = objectEvent as IAwake;
			if (iAwake == null)
			{
				return;
			}
			objectEvent.Set(disposer);
			iAwake.Awake();
		}

		public void Awake<P1>(ObjectDisposer disposer, P1 p1)
		{
			this.Add(disposer);

            IObjectEventFacade objectEvent;

            if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
			{
				throw new Exception($"{disposer.GetType().Name} not found awake1");
			}
			IAwake<P1> iAwake = objectEvent as IAwake<P1>;
			if (iAwake == null)
			{
				throw new Exception($"{disposer.GetType().Name} not found awake1");
			}
			objectEvent.Set(disposer);
			iAwake.Awake(p1);
		}

		public void Awake<P1, P2>(ObjectDisposer disposer, P1 p1, P2 p2)
		{
			this.Add(disposer);

            IObjectEventFacade objectEvent;

            if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
			{
				throw new Exception($"{disposer.GetType().Name} not found awake2");
			}
			IAwake<P1, P2> iAwake = objectEvent as IAwake<P1, P2>;
			if (iAwake == null)
			{
				throw new Exception($"{disposer.GetType().Name} not found awake2");
			}
			objectEvent.Set(disposer);
			iAwake.Awake(p1, p2);
		}

		public void Awake<P1, P2, P3>(ObjectDisposer disposer, P1 p1, P2 p2, P3 p3)
		{
			this.Add(disposer);

            IObjectEventFacade objectEvent;

            if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
			{
				throw new Exception($"{disposer.GetType().Name} not found awake3");
			}
			IAwake<P1, P2, P3> iAwake = objectEvent as IAwake<P1, P2, P3>;
			if (iAwake == null)
			{
				throw new Exception($"{disposer.GetType().Name} not found awake3");
			}
			objectEvent.Set(disposer);
			iAwake.Awake(p1, p2, p3);
		}

		public void Load()
		{
			unique.Clear();
			while (this.loaders.Count > 0)
			{
				ObjectDisposer disposer = this.loaders.Dequeue();
				if (disposer.Id == 0)
				{
					continue;
				}

				if (!this.unique.Add(disposer))
				{
					continue;
				}

                IObjectEventFacade objectEvent;

                if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
				{
					continue;
				}

				this.loaders2.Enqueue(disposer);

				ILoad iLoad = objectEvent as ILoad;
				if (iLoad == null)
				{
					continue;
				}
				objectEvent.Set(disposer);
				try
				{
					iLoad.Load();
				}
				catch (Exception e)
				{
					Logger.LogError(e.ToString());
				}
			}

			ObjectHelper.Swap(ref this.loaders, ref this.loaders2);
		}

		private void Start()
		{
			unique.Clear();
			while (this.starts.Count > 0)
			{
				ObjectDisposer disposer = this.starts.Dequeue();

				if (!this.unique.Add(disposer))
				{
					continue;
				}

                IObjectEventFacade objectEvent;

                if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
				{
					continue;
				}
				IStart iStart = objectEvent as IStart;
				if (iStart == null)
				{
					continue;
				}
				objectEvent.Set(disposer);
				iStart.Start();
			}
		}

		public void Update()
		{
			this.Start();

			unique.Clear();
			while (this.updates.Count > 0)
			{
				ObjectDisposer disposer = this.updates.Dequeue();
				if (disposer.IsDispose)
				{
					continue;
				}

				if (!this.unique.Add(disposer))
				{
					continue;
				}

                IObjectEventFacade objectEvent;

                if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectEvent))
				{
					continue;
				}

				this.updates2.Enqueue(disposer);

				IUpdate iUpdate = objectEvent as IUpdate;
				if (iUpdate == null)
				{
					continue;
				}
				objectEvent.Set(disposer);
				try
				{
					iUpdate.Update();
				}
				catch (Exception e)
				{
					Logger.LogError(e.ToString());
				}
			}

			ObjectHelper.Swap(ref this.updates, ref this.updates2);
		}

		public void LateUpdate()
		{
			unique.Clear();
			while (this.lateUpdates.Count > 0)
			{
				ObjectDisposer disposer = this.lateUpdates.Dequeue();
				if (disposer.IsDispose)
				{
					continue;
				}

				if (!this.unique.Add(disposer))
				{
					continue;
				}

				IObjectEventFacade objectSystem;
				if (!this.disposerEvents.TryGetValue(disposer.GetType(), out objectSystem))
				{
					continue;
				}

				this.lateUpdates2.Enqueue(disposer);

				ILateUpdate iLateUpdate = objectSystem as ILateUpdate;
				if (iLateUpdate == null)
				{
					continue;
				}
				objectSystem.Set(disposer);
				try
				{
					iLateUpdate.LateUpdate();
				}
				catch (Exception e)
				{
					Logger.LogError(e.ToString());
				}
			}

			ObjectHelper.Swap(ref this.lateUpdates, ref this.lateUpdates2);
		}
	}
}