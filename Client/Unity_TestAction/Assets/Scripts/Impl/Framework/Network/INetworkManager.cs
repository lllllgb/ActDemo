using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixFramework
{
    public interface INetworkManager
    {
        bool ConnectServer(int customSrvID, string ip, int port);

        bool Send(int customSrvID, object cmd);
    }
}
