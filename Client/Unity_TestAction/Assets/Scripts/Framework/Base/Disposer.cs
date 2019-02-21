using System;

namespace AosBaseFramework
{
	public abstract class Disposer : Object, IDisposable
	{
		public long InstanceId { get; protected set; }

        private bool mIsFromPool = false;
		public bool IsFromPool
        {
            get
            {
                return mIsFromPool;
            }
            set
            {
                mIsFromPool = value;

                if (!mIsFromPool)
                {
                    return;
                }

                if (this.InstanceId == 0)
                {
                    this.InstanceId = IdGenerater.GenerateId();
                }
            }
        }

        public bool IsDisposed
        {
            get
            {
                return InstanceId == 0;
            }
        }

        protected Disposer()
		{
			this.InstanceId = IdGenerater.GenerateId();
		}

		protected Disposer(long id)
		{
			this.InstanceId = id;
		}

		public virtual void Dispose()
		{
            if (IsDisposed)
            {
                return;
            }

            InstanceId = 0;

			if (IsFromPool)
			{
				Game.ObjectPool.Recycle(this);
			}
		}
	}
}