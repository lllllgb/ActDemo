using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AosHotfixFramework
{
    public class Service
    {
        private BufferWriter mBufferWriter = new BufferWriter();
        private Queue<BufferReader> mAsyncMsgQueue = new Queue<BufferReader>();
        private List<BufferReader> mSyncMsgList = new List<BufferReader>();
        private Dictionary<int, Connection> mID2ConnectionDict = new Dictionary<int, Connection>();
        private int mPort;
        private string mIP = string.Empty;

        public Action<int> ConnectSucceedHandle { get; set; }
        public Action<int> ConnectFailedHandle { get; set; }
        public Action<int> DisConnectedHandle { get; set; }

        protected virtual Connection Apply(int connectionID, Connection.Handle handle)
        {
            return null;
        }

        private bool Connect(Connection connection, string ip, int port, bool last)
        {
            mIP = ip;
            mPort = port;

            return connection.Connect(ip, port, last);
        }

        private bool Send(Connection connection, object cmd)
        {
            if (connection == null || mBufferWriter == null)
            {
                return false;
            }
            mBufferWriter.Clear();
            ushort num = 0;
            mBufferWriter.Write(num);
            CommandHelper.Serialize(mBufferWriter, cmd);
            num = (ushort)(mBufferWriter.stream.Length - 2);

            int high = num >> 8;
            int low = (num & 0xff) << 8;
            num = (ushort)(low | high);

            Array.Copy(BitConverter.GetBytes(num), mBufferWriter.stream.GetBuffer(), 2);
            //Logger.Log($"Send cmd = {cmd}");
            return connection.Send(mBufferWriter.stream.GetBuffer(), (int)mBufferWriter.stream.Length);
        }

        private int OnParse(byte[] data, int size)
        {
            //Logger.Log($"onParse data = {data} size = {size}");
            int i;
            ushort num;
            for (i = 0; i < size; i += 2 + num)
            {
                if (size < i + 2)
                {
                    break;
                }

                num = BitConverter.ToUInt16(data, i);
                int high = num >> 8;
                int low = (num & 0xff) << 8;
                num = (ushort)(low | high);

                if (size - i - 2 < num)
                {
                    break;
                }
                
                BufferReader bufferReader = new BufferReader();
                bufferReader.Load(data, i + 2, num);
                Queue<BufferReader> queue = mAsyncMsgQueue;
                Monitor.Enter(queue);
                try
                {
                    mAsyncMsgQueue.Enqueue(bufferReader);
                }
                finally
                {
                    Monitor.Exit(queue);
                }
            }
            return i;
        }

        private void Reset()
        {
            if (mAsyncMsgQueue != null)
            {
                Queue<BufferReader> queue = mAsyncMsgQueue;
                Monitor.Enter(queue);

                try
                {
                    mAsyncMsgQueue.Clear();
                }
                finally
                {
                    Monitor.Exit(queue);
                }
            }
        }


        //
        public bool Send(int customSrvID, object cmd)
        {
            Connection tmpConnection = null;

            if (!mID2ConnectionDict.TryGetValue(customSrvID, out tmpConnection))
            {
                return false;
            }

            return Send(tmpConnection, cmd);
        }

        //
        public void Process()
        {
            mSyncMsgList.Clear();
            Queue<BufferReader> queue = mAsyncMsgQueue;
            Monitor.Enter(queue);

            try
            {
                while (mAsyncMsgQueue.Count > 0)
                {
                    BufferReader bufferReader = mAsyncMsgQueue.Dequeue();
                    if (bufferReader != null)
                    {
                        mSyncMsgList.Add(bufferReader);
                    }
                }
            }
            finally
            {
                Monitor.Exit(queue);
            }

            int i = 0;
            int count = mSyncMsgList.Count;

            while (i < count)
            {
                CommandHelper.Process(mSyncMsgList[i]);
                i++;
            }

            mSyncMsgList.Clear();
        }

        public void Close()
        {
            Logger.Log("==================== Service::Close ====================");

            foreach (KeyValuePair<int, Connection> elem in mID2ConnectionDict)
            {
                elem.Value.Close();
            }

            mID2ConnectionDict.Clear();
            Reset();
        }

        public bool ConnectServer(int customSrvID, string ip, int port)
        {
            Connection tmpConnection = null;

            if (!mID2ConnectionDict.TryGetValue(customSrvID, out tmpConnection))
            {
                tmpConnection = Apply(customSrvID, OnParse);
                mID2ConnectionDict.Add(customSrvID, tmpConnection);
                tmpConnection.connectHandle += OnConnectHandle;
                tmpConnection.disconnectHandle += OnDisConnectHandle;
            }

            mIP = ip;
            mPort = port;
            tmpConnection.Close();
            return Connect(tmpConnection, ip, port, false);
        }

        private void OnConnectHandle(int customSrvID, object result)
        {
            if ((bool)result)
            {
                if (null != ConnectSucceedHandle)
                    ConnectSucceedHandle(customSrvID);
            }
            else
            {
                Logger.LogWarningFormat("连接服务器失败: {0}:{1}.", mIP, mPort);
                if (null != ConnectFailedHandle)
                    ConnectFailedHandle(customSrvID);
            }
        }

        private void OnDisConnectHandle(int customSrvID, object result)
        {
            if(null != DisConnectedHandle)
                DisConnectedHandle(customSrvID);
        }
    }
}
