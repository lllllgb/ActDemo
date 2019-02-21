using System;

namespace AosHotfixFramework
{
    public class TcpService : Service
    {
        protected override Connection Apply(int connectionID, Connection.Handle handle)
        {
            return new TcpConnect(connectionID, handle);
        }
    }
}