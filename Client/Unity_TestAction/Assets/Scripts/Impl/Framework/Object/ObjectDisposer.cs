using System;
using System.Collections.Generic;
using AosBaseFramework;

namespace AosHotfixFramework
{
    public class ObjectDisposer : IPoolObject
    {
        public long Id { get; set; }

        public bool IsFromPool { get; set; }

        public bool IsDispose { get; private set; }

        protected ObjectDisposer()
        {
            this.Id = IdGenerater.GenerateId();
        }

        public virtual void Dispose()
        {
            IsDispose = true;

            if (this.IsFromPool)
            {
                IObjectPoolManager tmpPoolMgr = GameModuleManager.GetModule<IObjectPoolManager>();

                if (null != tmpPoolMgr)
                {
                    var tmpObjPool = tmpPoolMgr.GetObjectPoolByType(this.GetType());
                    tmpObjPool.Unspawn(this);
                }
            }
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnSpawn()
        {
            IsDispose = false;
        }

        public virtual void OnUnspawn()
        {
            IsDispose = true;
        }

        public virtual void OnRelease()
        {
        }
    }
}
