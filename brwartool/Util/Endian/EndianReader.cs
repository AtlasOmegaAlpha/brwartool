using System.Text;

namespace System.IO
{
    public sealed class EndianReader : EndianStream
    {
        #region Initiators
        public EndianReader(Stream stream, Endianness endian)
        {
            if (stream == null)
                throw new NullReferenceException();

            this.disposed = false;
            this.position = 0;
            this.endian = endian;
            this.m = stream;
        }

        public EndianReader(byte[] data, Endianness endian)
        {
            if (data.Length < 0)
                throw new Exception("Array size cannot be less than or equal to 0");

            this.disposed = false;
            this.position = 0;
            this.endian = endian;
            this.m = new MemoryStream(data);
        }
        #endregion

        #region Private Functions
        private void Try(int length)
        {
            if (this.m == null)
                throw new NullReferenceException();

            if (this.position + length > this.m.Length)
                throw new EndOfStreamException();
        }

        private void FillBuffer(int count, int stride)
        {
            this.buffer = new byte[count];
            this.m.Read(this.buffer, 0, count);
            this.position += count;
            if (this.endian == Endianness.LittleEndian)
            {
                for (int i = 0; i < count; i += stride)
                {
                    Array.Reverse(this.buffer, i, stride);
                }
            }
        }
        #endregion

        #region Public Functions
        public byte ReadByte()
        {
            Try(sizeof(byte));
            FillBuffer(1, 1);
            return this.buffer[0];
        }

        public byte[] ReadBytes(int count)
        {
            Try(sizeof(byte) * count);
            FillBuffer(count, 1);
            return this.buffer;
        }

        public sbyte ReadSByte()
        {
            Try(sizeof(sbyte));
            FillBuffer(1, 1);
            unchecked { return (sbyte)this.buffer[0]; }
        }

        public sbyte[] ReadSBytes(int count)
        {
            Try(sizeof(sbyte) * count);
            FillBuffer(count, 1);
            sbyte[] temp = new sbyte[count];
            for (int i = 0; i < count; i++)
            {
                unchecked { temp[i] = (sbyte)this.buffer[i]; }
            }
            return temp;
        }

        public ushort ReadUInt16()
        {
            Try(sizeof(ushort));
            FillBuffer(2, 2);
            Array.Reverse(this.buffer);
            return BitConverter.ToUInt16(this.buffer, 0);
        }

        public ushort[] ReadUInt16s(int count)
        {
            Try(sizeof(ushort) * count);
            FillBuffer(2 * count, 2);
            ushort[] temp = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[2];
                Array.Copy(this.buffer, i * 2, temp2, 0, 2);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToUInt16(temp2, 0);
            }
            return temp;
        }

        public short ReadInt16()
        {
            Try(sizeof(short));
            FillBuffer(2, 2);
            Array.Reverse(this.buffer);
            return BitConverter.ToInt16(this.buffer, 0);
        }

        public short[] ReadInt16s(int count)
        {
            Try(sizeof(short) * count);
            FillBuffer(2 * count, 2);
            short[] temp = new short[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[2];
                Array.Copy(this.buffer, i * 2, temp2, 0, 2);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToInt16(temp2, 0);
            }
            return temp;
        }

        public uint ReadUInt24()
        {
            Try(3);
            FillBuffer(3, 3);
            return (uint)(this.buffer[0] << 16 | this.buffer[1] << 8 | this.buffer[2]);
        }

        public int ReadInt24()
        {
            Try(3);
            FillBuffer(3, 3);
            unchecked { return this.buffer[0] << 16 | this.buffer[1] << 8 | this.buffer[2]; }
        }

        public uint ReadUInt32()
        {
            Try(sizeof(uint));
            FillBuffer(4, 4);
            Array.Reverse(this.buffer);
            return BitConverter.ToUInt32(this.buffer, 0);
        }

        public uint[] ReadUInt32s(int count)
        {
            Try(sizeof(uint) * count);
            FillBuffer(4 * count, 4);
            uint[] temp = new uint[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[4];
                Array.Copy(this.buffer, i * 4, temp2, 0, 4);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToUInt32(temp2, 0);
            }
            return temp;
        }

        public int ReadInt32()
        {
            Try(sizeof(int));
            FillBuffer(4, 4);
            Array.Reverse(this.buffer);
            return BitConverter.ToInt32(this.buffer, 0);
        }

        public int[] ReadInt32s(int count)
        {
            Try(sizeof(int) * count);
            FillBuffer(4 * count, 4);
            int[] temp = new int[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[4];
                Array.Copy(this.buffer, i * 4, temp2, 0, 4);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToInt32(temp2, 0);
            }
            return temp;
        }

        public ulong ReadUInt64()
        {
            Try(sizeof(ulong));
            FillBuffer(8, 8);
            Array.Reverse(this.buffer);
            return BitConverter.ToUInt64(this.buffer, 0);
        }

        public ulong[] ReadUInt64s(int count)
        {
            Try(sizeof(ulong) * count);
            FillBuffer(8 * count, 8);
            ulong[] temp = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[8];
                Array.Copy(this.buffer, i * 8, temp2, 0, 8);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToUInt64(temp2, 0);
            }
            return temp;
        }

        public long ReadInt64()
        {
            Try(sizeof(long));
            FillBuffer(8, 8);
            Array.Reverse(this.buffer);
            return BitConverter.ToInt64(this.buffer, 0);
        }

        public long[] ReadInt64s(int count)
        {
            Try(sizeof(long) * count);
            FillBuffer(8 * count, 8);
            long[] temp = new long[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[8];
                Array.Copy(this.buffer, i * 8, temp2, 0, 8);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToInt64(temp2, 0);
            }
            return temp;
        }

        public float ReadSingle()
        {
            Try(sizeof(float));
            FillBuffer(4, 4);
            Array.Reverse(this.buffer);
            return BitConverter.ToSingle(this.buffer, 0);
        }

        public float ReadFloat()
        {
            return ReadSingle();
        }

        public float[] ReadSingles(int count)
        {
            Try(sizeof(float) * count);
            FillBuffer(4 * count, 4);
            float[] temp = new float[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[4];
                Array.Copy(this.buffer, i * 4, temp2, 0, 4);
                Array.Reverse(temp2);
                temp[i] = BitConverter.ToSingle(temp2, 0);
            }
            return temp;
        }

        public float[] ReadFloats(int count)
        {
            return ReadSingles(count);
        }

        public double ReadDouble()
        {
            Try(sizeof(double));
            FillBuffer(8, 8);
            Array.Reverse(this.buffer);
            return BitConverter.ToDouble(this.buffer, 0);
        }

        public double[] ReadDouble(int count)
        {
            Try(sizeof(double));
            FillBuffer(8 * count, 8);
            double[] temp = new double[count];
            for (int i = 0; i < count; i++)
            {
                byte[] temp2 = new byte[8];
                Array.Copy(this.buffer, i * 8, temp2, 0, 8);
                temp[i] = BitConverter.ToSingle(temp2, 0);
            }
            return temp;
        }

        public string ReadString(int count)
        {
            Try(count);
            FillBuffer(count, 1);
            return Encoding.Default.GetString(this.buffer);
        }

        public string ReadString(int count, Encoding en)
        {
            Try(count * GetEncodingSize(en));
            FillBuffer(count * GetEncodingSize(en), GetEncodingSize(en));
            return en.GetString(this.buffer);
        }

        public string ReadStringNT()
        {
            List<byte> temp = new List<byte>();
            do
            {
                FillBuffer(1, 1);
                temp.Add(this.buffer[0]);
            }
            while (this.buffer[0] != 0);
            temp.RemoveAt(temp.Count - 1);
            return Encoding.Default.GetString(temp.ToArray());
        }

        public string ReadStringNT(Encoding en)
        {
            int encodingSize = GetEncodingSize(en);
            List<byte> temp = new List<byte>();
            do
            {
                FillBuffer(encodingSize, encodingSize);
                for (int i = 0; i < encodingSize; i++)
                {
                    temp.Add(this.buffer[i]);
                }
            }
            while (this.buffer[encodingSize - 1] != 0);
            for (int i = 0; i < encodingSize; i++)
            {
                temp.RemoveAt(temp.Count - 1);
            }
            return en.GetString(temp.ToArray());
        }

        public char ReadChar()
        {
            Try(1);
            FillBuffer(1, 1);
            return Encoding.Default.GetChars(this.buffer)[0];
        }

        public char ReadChar(Encoding en)
        {
            Try(GetEncodingSize(en));
            FillBuffer(GetEncodingSize(en), GetEncodingSize(en));
            return en.GetString(this.buffer)[0];
        }

        public char[] ReadChars(int count)
        {
            Try(count);
            FillBuffer(count, 1);
            return Encoding.Default.GetChars(this.buffer);
        }

        public char[] ReadChars(int count, Encoding en)
        {
            Try(count * GetEncodingSize(en));
            FillBuffer(count * GetEncodingSize(en), GetEncodingSize(en));
            return en.GetChars(this.buffer);
        }
        #endregion
    }
}
