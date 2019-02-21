using System.Collections;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    internal sealed class NetworkManager : GameModuleBase, INetworkManager
    {
        Service mTcpService;

        public NetworkManager()
        {
            CommandHelper.Init();
            mTcpService = new TcpService();
            mTcpService.ConnectSucceedHandle = OnConnectSucceedHandle;
            mTcpService.ConnectFailedHandle = OnConnectFailHandle;
            mTcpService.DisConnectedHandle = OnDisConnectHandle;
            
        }

        internal override int Priority
        {
            get
            {
                return 200;
            }
        }
        

        internal override void Update(float deltaTime)
        {
            mTcpService.Process();
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
            mTcpService.Close();
        }

        public bool ConnectServer(int customSrvID, string ip, int port)
        {
            return mTcpService.ConnectServer(customSrvID, ip, port);
        }

        public bool Send(int customSrvID, object cmd)
        {
            return mTcpService.Send(customSrvID, cmd);
        }

        private void OnConnectSucceedHandle(int customSrvID)
        {
            var tmpArg = ReferencePool.Fetch<ServiceConnectSucceedEventArgs>();
            tmpArg.CustomServerID = customSrvID;

            IEventManager tmpEventMgr = GameModuleManager.GetModule<IEventManager>();
            if (null != tmpEventMgr)
            {
                tmpEventMgr.FireNow(this, tmpArg);
            }
        }

        private void OnConnectFailHandle(int customSrvID)
        {
            var tmpArg = ReferencePool.Fetch<ServiceConnectFailedEventArgs>();
            tmpArg.CustomServerID = customSrvID;

            IEventManager tmpEventMgr = GameModuleManager.GetModule<IEventManager>();
            if (null != tmpEventMgr)
            {
                tmpEventMgr.FireNow(this, tmpArg);
            }
        }

        private void OnDisConnectHandle(int customSrvID)
        {
            var tmpArg = ReferencePool.Fetch<ServiceDisConnectEventArgs>();
            tmpArg.CustomServerID = customSrvID;

            IEventManager tmpEventMgr = GameModuleManager.GetModule<IEventManager>();
            if (null != tmpEventMgr)
            {
                tmpEventMgr.FireNow(this, tmpArg);
            }
        }
    }
}
