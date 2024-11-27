using System.Text;

namespace System.IO
{
    public sealed class EndianWriter : EndianStream
    {
        #region Initiators
        public EndianWriter(Stream stream, Endianness endian)
        {
            if (stream == null)
                throw new NullReferenceException();

            this.disposed = false;
            this.position = 0;
            this.m = stream;
            this.endian = endian;
        }
        #endregion

        #region Private Functions
        private void CreateBuffer(int size)
        {
            this.buffer = new byte[size];
        }

        private void WriteBuffer(int count, int stride)
        {
            this.position += count;
            if (this.endian == Endianness.LittleEndian)
            {
                for (int i = 0; i < count; i += stride)
                {
                    Array.Reverse(this.buffer, i, stride);
                }
            }
            m.Write(this.buffer, 0, count);
        }
        #endregion

        #region Public Functions
        public void WriteByte(byte data)
        {
            CreateBuffer(sizeof(byte));
            this.buffer[0] = data;
            WriteBuffer(1, 1);
        }

        public void WriteBytes(byte[] data)
        {
            CreateBuffer(data.Length);
            this.buffer = data;
            WriteBuffer(data.Length, 1);
        }

        public void WriteSByte(sbyte data)
        {
            CreateBuffer(sizeof(sbyte));
            unchecked { this.buffer[0] = (byte)data; }
            WriteBuffer(1, 1);
        }

        public void WriteSBytes(sbyte[] data)
        {
            CreateBuffer(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                unchecked { this.buffer[i] = (byte)data[i]; }
            }
            WriteBuffer(data.Length, 1);
        }

        public void WriteUInt16(ushort data)
        {
            CreateBuffer(sizeof(ushort));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(2, 2);
        }

        public void WriteUInt16s(ushort[] data)
        {
            CreateBuffer(sizeof(ushort) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 2, 2);
            }
            WriteBuffer(2 * data.Length, 2);
        }

        public void WriteInt16(short data)
        {
            CreateBuffer(sizeof(short));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(2, 2);
        }

        public void WriteInt16s(short[] data)
        {
            CreateBuffer(sizeof(short) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 2, 2);
            }
            WriteBuffer(2 * data.Length, 2);
        }

        public void WriteUInt24(uint data)
        {
            CreateBuffer(3);
            byte[] temp = new byte[3] { (byte)((data >> 16) & 0xFF), (byte)((data >> 8) & 0xFF), (byte)(data & 0xFF) };
            this.buffer = temp;
            WriteBuffer(3, 3);
        }

        public void WriteUInt32(uint data)
        {
            CreateBuffer(sizeof(uint));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(4, 4);
        }

        public void WriteUInt32s(uint[] data)
        {
            CreateBuffer(sizeof(uint) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 4, 4);
            }
            WriteBuffer(4 * data.Length, 4);
        }

        public void WriteInt32(int data)
        {
            CreateBuffer(sizeof(int));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(4, 4);
        }

        public void WriteInt32s(int[] data)
        {
            CreateBuffer(sizeof(int) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 4, 4);
            }
            WriteBuffer(4 * data.Length, 4);
        }

        public void WriteUInt64(ulong data)
        {
            CreateBuffer(sizeof(ulong));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(8, 8);
        }

        public void WriteUInt64s(ulong[] data)
        {
            CreateBuffer(sizeof(ulong) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 8, 8);
            }
            WriteBuffer(8 * data.Length, 8);
        }

        public void WriteInt64(long data)
        {
            CreateBuffer(sizeof(long));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(8, 8);
        }

        public void WriteInt64s(long[] data)
        {
            CreateBuffer(sizeof(long) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 8, 8);
            }
            WriteBuffer(8 * data.Length, 8);
        }

        public void WriteSingle(float data)
        {
            CreateBuffer(sizeof(float));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(4, 4);
        }

        public void WriteFloat(float data)
        {
            WriteSingle(data);
        }

        public void WriteSingles(float[] data)
        {
            CreateBuffer(sizeof(float) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 4, 4);
            }
            WriteBuffer(4 * data.Length, 4);
        }

        public void WriteFloats(float[] data)
        {
            WriteSingles(data);
        }

        public void WriteDouble(double data)
        {
            CreateBuffer(sizeof(double));
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            this.buffer = temp;
            WriteBuffer(8, 8);
        }

        public void WriteDoubles(double[] data)
        {
            CreateBuffer(sizeof(double) * data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(data[i]);
                Array.Reverse(temp);
                Array.Copy(temp, 0, this.buffer, i * 8, 8);
            }
            WriteBuffer(8 * data.Length, 8);
        }

        public void WriteString(string data)
        {
            CreateBuffer(data.Length * GetEncodingSize(Encoding.Default));
            this.buffer = Encoding.Default.GetBytes(data.ToCharArray());
            WriteBuffer(data.Length, GetEncodingSize(Encoding.Default));
        }

        public void WriteString(string data, Encoding en)
        {
            CreateBuffer(data.Length * GetEncodingSize(en));
            this.buffer = en.GetBytes(data.ToCharArray());
            WriteBuffer(data.Length * GetEncodingSize(en), GetEncodingSize(en));
        }

        public void WriteStringNT(string data)
        {
            WriteString(data);
            this.buffer = new byte[1];
            WriteBuffer(1, 1);
        }

        public void WriteStringNT(string data, Encoding en)
        {
            WriteString(data, en);
            this.buffer = new byte[1];
            WriteBuffer(1, 1);
        }

        public void WriteStrings(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                WriteString(data[i]);
            }
        }

        public void WriteStrings(ICollection<string> data)
        {
            string[] data2 = data.ToArray();
            WriteStrings(data2);
        }

        public void WriteStrings(ICollection<string> data, Encoding en)
        {
            string[] data2 = data.ToArray();
            WriteStrings(data2, en);
        }

        public void WriteChar(char data)
        {
            CreateBuffer(GetEncodingSize(Encoding.Default));
            this.buffer = Encoding.Default.GetBytes(new char[1] { data });
            WriteBuffer(GetEncodingSize(Encoding.Default), GetEncodingSize(Encoding.Default));
        }

        public void WriteChar(char data, Encoding en)
        {
            CreateBuffer(GetEncodingSize(en));
            this.buffer = Encoding.Default.GetBytes(new char[1] { data });
            WriteBuffer(GetEncodingSize(en), GetEncodingSize(en));
        }

        public void WriteChars(char[] data)
        {
            CreateBuffer(data.Length * GetEncodingSize(Encoding.Default));
            this.buffer = Encoding.Default.GetBytes(data);
            WriteBuffer(data.Length, GetEncodingSize(Encoding.Default));
        }

        public void WriteChars(char[] data, Encoding en)
        {
            CreateBuffer(data.Length * GetEncodingSize(en));
            this.buffer = en.GetBytes(data);
            WriteBuffer(data.Length * GetEncodingSize(en), GetEncodingSize(en));
        }
        #endregion
    }
}
