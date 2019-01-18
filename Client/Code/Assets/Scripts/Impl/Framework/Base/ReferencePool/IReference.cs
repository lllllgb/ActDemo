using AosBaseFramework;

namespace AosHotfixFramework
{
    public interface IReference
    {
        bool IsFromPool { get; set; }

        void Dispose();
    }

    public class ReferenceDisposer : IReference
    {
        public long Id { get; set; }

        public bool IsFromPool { get; set; }

        protected ReferenceDisposer()
        {
            this.Id = IdGenerater.GenerateId();
        }

        public virtual void Dispose()
        {
            if (this.IsFromPool)
            {
                ReferencePool.Recycle(this);
            }
        }
    }
}
