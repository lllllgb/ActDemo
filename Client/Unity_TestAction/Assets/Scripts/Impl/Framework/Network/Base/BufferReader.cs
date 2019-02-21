using System;
using System.IO;


public sealed class BufferReader
{
    private MemoryStream m_stream = null;
    private BinaryReader m_reader = null;
    
    public MemoryStream stream
    {
        get { return m_stream; }
    }

    public int position
    {
        get { return (int)m_stream.Position; }
    }

    public BufferReader()
    {
        m_stream = new MemoryStream();
        m_reader = new BinaryReader(m_stream);
    }

    public void Load(byte[] data)
    {
        Load(data, 0, data.Length);
    }

    public void Load(byte[] data, int index, int size)
    {
        m_stream.Write(data, index, size);
        m_stream.Position = 0;
    }

    public BufferReader Read(ref sbyte value)
    {
        value = (sbyte)m_stream.ReadByte();
        return this;
    }

    public BufferReader Read(ref byte value)
    {
        value = (byte)m_stream.ReadByte();
        return this;
    }

    public BufferReader Read(ref short value)
    {
        value = m_reader.ReadInt16();
        return this;
    }

    public BufferReader Read(ref ushort value)
    {
        value = m_reader.ReadUInt16();
        return this;
    }

    public BufferReader Read(ref int value)
    {
        value = m_reader.ReadInt32();
        return this;
    }

    public BufferReader Read(ref uint value)
    {
        value = m_reader.ReadUInt32();
        return this;
    }

    public BufferReader Read(ref long value)
    {
        value = m_reader.ReadInt64();
        return this;
    }

    public BufferReader Read(ref ulong value)
    {
        value = m_reader.ReadUInt64();
        return this;
    }

    public BufferReader Read(ref float value)
    {
        value = m_reader.ReadSingle();
        return this;
    }

    public BufferReader Read(ref double value)
    {
        value = m_reader.ReadDouble();
        return this;
    }

    public BufferReader Read(ref byte[] value)
    {
        long num = m_stream.Length - m_stream.Position;
        value = new byte[num];
        m_reader.Read(value, 0, value.Length);
        return this;
    }
}