using System.Text;

namespace System.IO
{
    public abstract class EndianStream : IDisposable
    {
        public byte[] buffer { get; set; }
        public Stream m { get; set; }
        public Endianness endian { get; set; }
        public long position { get; set; }
        public bool disposed { get; set; }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
                return;

            GC.SuppressFinalize(this);
            if (this.m != null)
            {
                this.m.Close();
                this.m = null;
            }
            this.position = 0;
            this.disposed = true;
        }

        public static int GetEncodingSize(Encoding en)
        {
            switch (en.EncodingName)
            {
                case "Unicode":
                case "BigEndianUnicode":
                    return 2;

                case "UTF32":
                    return 4;

                default:
                    return 1;
            }
        }

        public Stream Stream
        {
            get
            {
                if (this.m == null)
                    throw new NullReferenceException();

                return this.m;
            }
        }
        public Endianness Endian
        {
            get { return this.endian; }
            set { this.endian = value; }
        }

        public long Position
        {
            get
            {
                if (this.m == null)
                    throw new NullReferenceException();

                return this.position;
            }
            set
            {
                if (this.m == null)
                    throw new NullReferenceException();

                if (value < 0)
                    throw new ArgumentException("The position can't be negative");

                this.m.Position = value;
                this.position = value;
            }
        }

        public long StreamLength
        {
            get
            {
                if (this.m == null)
                    throw new NullReferenceException();

                return this.m.Length;
            }
        }

        public void AlignPosition(long bytes)
        {
            while (Position % bytes != 0)
                Position++;
        }
    }
}