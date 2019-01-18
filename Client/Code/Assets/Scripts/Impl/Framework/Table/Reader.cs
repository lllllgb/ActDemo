using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JW_Table
{
    public class Reader
    {
        public Reader(byte[] buffer, int offset, int length)
        {
            m_buffer = buffer;
            m_stream = new MemoryStream(buffer, offset, length);
            m_binaryReader = new BinaryReader(m_stream);
        }

        byte[] m_buffer = null;
        MemoryStream m_stream = null;
        BinaryReader m_binaryReader = null;

        public void Load(byte[] buffer, int offset, int length)
        {
            m_stream.Read(buffer, offset, length);
            m_buffer = buffer;
        }

        public BinaryReader getReader()
        {
            return m_binaryReader;
        }

        public MemoryStream stream
        {
            get { return m_stream; }
        }

        public int ReadInt32_()
        {
            int value = m_binaryReader.ReadInt32();
            return value;
        }

        public uint ReadUInt32_()
        {
            uint value = m_binaryReader.ReadUInt32();
            return value;
        }

        public bool ReadBoolean()
        {
            uint temp = ReadUInt32Variant();
            bool value = temp == 1;
            return value;
        }

        public List<bool> readRepeatedBoolean(List<bool> list)
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                uint temp = ReadUInt32Variant();
                list.Add(temp == 1);
            }
            return list;
        }

        public int ReadEnum()
        {
            int value = ReadInt32Variant();
            return value;
        }

        public int ReadInt32()
        {
            int value = ReadInt32Variant();
            return value;
        }

        public List<int> ReadRepeatedInt32(List<int> list)
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                int temp = ReadInt32Variant();
                list.Add(temp);
            }
            return list;
        }

        public long ReadInt64()
        {
            long value = ReadInt64Variant();
            return value;
        }

        public List<long> ReadRepeatedInt64(List<long> list)
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                long temp = ReadInt64Variant();
                list.Add(temp);
            }
            return list;
        }

        public float ReadFloat()
        {
            float value = m_binaryReader.ReadSingle();
            return value;
        }

        public List<float> readRepeatedFloat(List<float> list)
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                float temp = 0;
                temp = m_binaryReader.ReadSingle();
                list.Add(temp);
            }
            return list;
        }

        public string ReadString()
        {
            int pos = ReadInt32_();
            int count = 0;
            while (0 < m_buffer[pos + count])
                ++count;
            Encoding code = Encoding.GetEncoding("utf-8");
            return code.GetString(m_buffer, pos, count);
        }

        public List<string> ReadRepeatedString(List<string> list)
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                string temp = ReadString();
                list.Add(temp);
            }
            return list;
        }

        public T ReadItem<T>() where T : Binary
        {
            T value = System.Activator.CreateInstance<T>();
            value.Read(this);
            return value;
        }

        public List<T> ReadRepeatedItem<T>(List<T> list) where T : Binary
        {
            uint count = ReadUInt32Variant();
            for (int i = 0; i < count; i++)
            {
                T temp = this.ReadItem<T>();
                list.Add(temp);
            }
            return list;
        }

        #region Variant

        public Int32 ReadInt32Variant()
        {
            return (Int32)ReadUInt32Variant();
        }

        public UInt32 ReadUInt32Variant()
        {
            UInt32 value = 0;
            byte tempByte = 0;
            int index = 0;
            do
            {
                tempByte = m_binaryReader.ReadByte();
                UInt32 temp1 = (UInt32)((tempByte & 0x7F) << index);  // 0x7F (1<<7)-1  127
                value |= temp1;
                index += 7;
            }
            while ((tempByte >> 7) > 0);
            return value;
        }

        public Int64 ReadInt64Variant()
        {
            return (Int64)ReadUInt64Variant();
        }

        public UInt64 ReadUInt64Variant()
        {
            UInt64 value = 0;
            byte tempByte = 0;
            int index = 0;
            do
            {
                tempByte = m_binaryReader.ReadByte();
                UInt64 temp1 = (UInt64)((tempByte & 0x7F) << index);  // 0x7F (1<<7)-1  127
                value |= temp1;
                index += 7;
            }
            while ((tempByte >> 7) > 0);
            return value;
        }

        #endregion

        public void Clear()
        {
            m_stream.SetLength(0);
            m_buffer = null;
        }

        public void Close()
        {
            m_binaryReader.Close();
            m_stream.Close();
        }
    }
}
