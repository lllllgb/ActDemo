using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JW_Table
{
    public class TableManager<T> where T : Binary
    {
        private List<T> m_list = new List<T>();

        protected TableManager()
        {
        }

        public int Size
        {
            get { return m_list.Count; }
        }

        public T Get(int index)
        {
            return index < Size ? m_list[index] : default(T);
        }

        protected T FindInternal(Int64 key)
        {
            int index = TableUtility.BinarySearch(m_list, key);
            return index == -1 ? default(T) : Get(index);
        }

        protected bool Load(string path, string filename, uint version)
        {
            try
            {
                byte[] tmpBytes = AosBaseFramework.FileHelper.LoadBytesFile(filename);
                return Load(tmpBytes, version, filename);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public bool Load(byte[] array, uint version, string fileName)
        {
            Reader reader = new Reader(array, 0, array.Length);
            uint tblVersion = reader.ReadUInt32_();

            if (tblVersion != version)
            {
                //版本不一致
                Logger.LogError($"配置表版本不一致 name = {fileName}");
                return false;
            }

            //行描述
            uint index = reader.ReadUInt32_();
            reader.stream.Position += index;

            m_list = reader.ReadRepeatedItem(m_list);

            return true;
        }
    }
}
