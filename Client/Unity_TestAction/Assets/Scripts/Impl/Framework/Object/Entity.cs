using System;
using System.Collections.Generic;
using System.Linq;
using AosBaseFramework;

namespace AosHotfixFramework
{
	public class Entity : Component
	{
		private readonly Dictionary<Type, Component> componentDict;

		protected Entity()
		{
			this.Id = IdGenerater.GenerateId();
			this.componentDict = new Dictionary<Type, Component>();
		}

		protected Entity(long id)
		{
			this.Id = id;
			this.componentDict = new Dictionary<Type, Component>();
		}

		public override void Dispose()
		{
			base.Dispose();

			foreach (Component component in this.GetComponents())
			{
				try
				{
					component.Dispose();
				}
				catch (Exception e)
				{
					Logger.LogError(e.ToString());
				}
			}
            
			this.componentDict.Clear();
		}

		public K AddComponent<K>() where K : Component, new()
		{
			K component = ComponentFactory.Create<K>(this);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1>(P1 p1) where K : Component, new()
		{
			K component = ComponentFactory.Create<K, P1>(this, p1);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}
            
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
		{
			K component = ComponentFactory.Create<K, P1, P2>(this, p1, p2);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}
            
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
		{
			K component = ComponentFactory.Create<K, P1, P2, P3>(this, p1, p2, p3);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}
            
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public void RemoveComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof(K), out component))
			{
				return;
			}
            
			this.componentDict.Remove(typeof(K));

			component.Dispose();
		}

		public void RemoveComponent(Type type)
		{
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return;
			}
            
			this.componentDict.Remove(type);

			component.Dispose();
		}

		public K GetComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof(K), out component))
			{
				return default(K);
			}
			return (K)component;
		}

		public Component[] GetComponents()
		{
			return this.componentDict.Values.ToArray();
		}
	}
}