using System;
using System.IO;


public sealed class BufferWriter
{
    private MemoryStream m_stream = null;
    private BinaryWriter m_writer = null;

    public MemoryStream stream
    {
        get { return m_stream; }
    }

    public BufferWriter()
    {
        m_stream = new MemoryStream();
        m_writer = new BinaryWriter(m_stream);
    }

    public BufferWriter(int size)
    {
        m_stream = new MemoryStream(size);
        m_writer = new BinaryWriter(m_stream);
    }

    public BufferWriter Write(sbyte value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(byte value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(short value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(ushort value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(int value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(uint value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(long value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(ulong value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(float value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(double value)
    {
        m_writer.Write(value);
        return this;
    }

    public BufferWriter Write(byte[] value)
    {
        Write((ushort)value.Length);
        m_writer.Write(value);
        return this;
    }

    public void Clear()
    {
        m_stream.Position = 0;
        m_stream.SetLength(0);
    }
}