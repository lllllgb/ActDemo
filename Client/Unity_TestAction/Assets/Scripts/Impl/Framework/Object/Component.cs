
namespace AosHotfixFramework
{
	public abstract class Component : ObjectDisposer
	{
		public Entity Parent { get; set; }

		public T GetParent<T>() where T : Entity
		{
			return this.Parent as T;
		}

		protected Component()
		{
			this.Id = 1;
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}