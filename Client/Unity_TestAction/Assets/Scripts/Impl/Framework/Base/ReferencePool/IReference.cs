using AosBaseFramework;

namespace AosHotfixFramework
{
    public interface IReference
    {
        void OnInit();
        void Dispose();
    }

    public class ReferenceDisposer : IReference
    {
        public long Id { get; set; }

        protected ReferenceDisposer()
        {
            this.Id = IdGenerater.GenerateId();
        }

        public virtual void OnInit()
        {
        }

        public virtual void Dispose()
        {
            ReferencePool.Recycle(this);
        }
    }
}
