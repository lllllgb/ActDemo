using System;

namespace AosHotfixFramework
{
    public class Connection
    {
        public delegate void EventHandle(int connectionID, object param);
        public delegate int Handle(byte[] data, int size);
        public const int MAX_BUFFER_SIZE = 65535;
        public const int MAX_PACKET_SIZE = 65531;

        private Handle m_handle = null;
        private EventHandle m_connected = null;
        private EventHandle m_disconnect = null;

        protected int m_pos = 0;
        protected int m_total_recv_length = 0;
        protected int m_total_send_length = 0;
        protected byte[] m_buffer = new byte[MAX_BUFFER_SIZE];

        protected int mID = 0;

        protected Handle handle
        {
            get { return m_handle; }
        }

        public EventHandle connectHandle
        {
            get { return m_connected; }
            set { m_connected = value; }
        }

        public EventHandle disconnectHandle
        {
            get { return m_disconnect; }
            set { m_disconnect = value; }
        }

        protected Connection(int id, Handle handle)
        {
            mID = id;
            m_handle = handle;
        }

        public virtual void Close()
        {

        }

        public virtual void Break()
        {

        }

        public virtual bool Connect(string hostName, int port, bool last)
        {
            return false;
        }

        public virtual bool Send(byte[] buffer, int size)
        {
            return false;
        }

        protected void OnConnected()
        {
            if (m_connected != null)
            {
                m_connected(mID, true);
            }
        }

        protected void OnConnectedFailed()
        {
            if (m_connected != null)
            {
                m_connected(mID, false);
            }
        }

        protected void OnDisconnected(object param)
        {
            if (m_disconnect != null)
            {
                m_disconnect(mID, param);
            }
        }
    }
}