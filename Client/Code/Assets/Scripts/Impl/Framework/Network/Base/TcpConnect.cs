using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace AosHotfixFramework
{
    public class TcpConnect : Connection
    {
        public enum State
        {
            Initliaze = 1,
            Connected = 2,
            ConnectFailed = 3,
            DisconnectTimeOut = 4,
            DisconnectRecvError = 5,
            DisconnectSendError = 6,
            Close = 7,
        }

        private Socket m_socket = null;
        private IAsyncResult m_recv = null;
        private IAsyncResult m_connect = null;
        private State m_state = State.Initliaze;

        public State state
        {
            get { return m_state; }
        }

        public TcpConnect(int connectionID, Handle handle)
            : base(connectionID, handle)
        {

        }

        private void Reset()
        {
            m_pos = 0;
            m_total_recv_length = 0;
            m_total_send_length = 0;
            m_state = State.Initliaze;
        }

        public override void Break()
        {
            if (m_socket != null)
            {
                //if (m_socket.Connected)
                {
                    m_socket.Shutdown(SocketShutdown.Both);
                    m_socket.Close();
                }

                m_socket = null;
            }
        }

        public override bool Connect(string hostName, int port, bool last)
        {
            if (m_connect != null)
            {
                return true;
            }

            try
            {
                Socket socket = null;
#if UNITY_IOS && !UNITY_EDITOR
            string newHostName = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            IOSIPSwitchHelper.GetIPType(hostName, port.ToString(), out newHostName, out newAddressFamily);
            if (!string.IsNullOrEmpty(newHostName))
            {
                hostName = newHostName;
            }
            socket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
#else
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#endif

                if (socket == null)
                {
                    throw new Exception(string.Format("------ TcpConnect create socket fail, host, port = {0}, {1}", hostName, port));
                }
                //UnityEngine.Debug.LogErrorFormat("心跳包端口hostName:{0}, port{1}", hostName, port);
                m_connect = socket.BeginConnect(hostName, port, OnConnect, socket);
                bool flag = m_connect.AsyncWaitHandle.WaitOne(5000, false);
                m_connect.AsyncWaitHandle.Close();
                //UnityEngine.Debug.LogErrorFormat("心跳包:{0} connect:{1}", flag, m_connect.ToString());
                if (flag)
                {
                    Socket socket2 = (Socket)m_connect.AsyncState;
                    socket2.EndConnect(m_connect);
                    m_connect = null;
                    Reset();

#if !UNITY_IOS
                    if (socket != null)
                    {
                        socket.Blocking = false;
                        socket.ReceiveTimeout = 6000;
                        socket.SendTimeout = 6000;
                        socket.SendBufferSize = 40000;
                    }
#else
                socket.Blocking = true;
#endif
                    m_socket = socket;

                    OnConnected();
                    Receive();
                    return true;
                }
                if (socket != null)
                {
                    //UnityEngine.Debug.LogErrorFormat("socket是否连接状态:socket.Connected{0} ", socket.Connected);
                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    socket.Close();
                }

                m_connect = null;
                if (!last)
                    return false;

                throw new Exception("-------- TcpConnect wait failed");
            }
            catch (Exception ex)
            {
                Logger.LogWarningFormat("-------- TcpConnect connect failed, host, port, msg = {0}, {1}, {2}",
                    hostName, port, ex.Message);

                //UnityEngine.Debug.LogErrorFormat("-------- TcpConnect connect failed, host, port, msg = {0}, {1}, {2}",
                //hostName, port, ex.Message);

                m_state = State.ConnectFailed;
                CloseAndJoin();
                base.OnConnectedFailed();
            }

            return false;
        }

        public override void Close()
        {
            m_state = State.Close;
            CloseAndJoin();
        }

        private void Disconnect(State state = State.Close)
        {
            if (m_state <= State.Connected)
            {
                m_state = state;
                CloseAndJoin();
                OnDisconnected(m_state);
            }
        }

        public void OnConnect(IAsyncResult ar)
        {

        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                ar.AsyncWaitHandle.Close();
                m_recv = null;
                Socket socket = (Socket)ar.AsyncState;
                if (socket != null && socket.Connected)
                {
                    int num = socket.EndReceive(ar);
                    if (num <= 0)
                    {
                        Logger.LogWarning("-------- TcpConnect recv 0 bytes from the socket, the connect bould break");
                        Disconnect(State.DisconnectRecvError);
                    }
                    else
                    {
                        m_pos += num;
                        if (handle != null)
                        {
                            int num2 = handle(m_buffer, m_pos);
                            int num3 = m_pos - num2;
                            if (num3 > 0)
                            {
                                Array.Copy(m_buffer, num2, m_buffer, 0, m_pos - num2);
                            }
                            m_pos = num3;
                        }
                        m_total_recv_length += num;
                        Receive();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarningFormat("--------- TcpConnect OnReceive exception, msg = {0}", ex.Message);
                Disconnect(State.DisconnectRecvError);
            }
        }

        private bool Receive()
        {
            try
            {
                m_recv = m_socket.BeginReceive(m_buffer, m_pos, MAX_BUFFER_SIZE - m_pos, 0, new AsyncCallback(OnReceive), m_socket);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarningFormat("-------- TcpConnect receive error, invalid operation : {0}", ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                Logger.LogWarningFormat("----------- TcpConnect receive error, invalid operation : {0}", ex.Message);
                return false;
            }

            return true;
        }

        public override bool Send(byte[] buffer, int size)
        {
            if (m_socket == null || !m_socket.Connected)
                return false;

            try
            {
                int num = 0;
                do
                {
                    int num2 = m_socket.Send(buffer, num, size - num, 0);
                    num += num2;
                }
                while (num < size);
                m_total_send_length += size;
            }
            catch (Exception ex)
            {
                Logger.LogWarningFormat("---------- TcpConnect send error, msg = {0}", ex.Message);
                Disconnect(State.DisconnectSendError);
                return false;
            }

            return true;
        }

        private void CloseAndJoin()
        {
            try
            {
                Break();
            }
            catch (Exception ex)
            {
                Logger.LogWarningFormat("---------- TcpConnect close socket failed, msg = {0}", ex.Message);
            }

            if (m_recv != null)
            {
                m_recv.AsyncWaitHandle.Close();
                m_recv = null;
            }
            if (m_connect != null)
            {
                m_connect.AsyncWaitHandle.Close();
                m_connect = null;
            }

            m_pos = 0;
            m_total_recv_length = 0;
            m_total_send_length = 0;
        }
    }
}