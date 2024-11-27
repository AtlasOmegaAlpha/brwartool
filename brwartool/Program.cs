using System.Diagnostics;
using System.Runtime.InteropServices;

namespace brwartool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }

            string action = args[0].ToLower();
            if (action.Equals("extract"))
                Extract(args[1], args[2]);
            else if (action.Equals("create"))
                Create(args[1], args[2]);
        }

        static void PrintUsage()
        {
            Console.WriteLine("brwartool extract [file.brwar] [folder]");
            Console.WriteLine("brwartool create [folder] [file.brwar]");
        }

        static void Extract(string inputFile, string outputFolder)
        {
            EndianReader reader = new EndianReader(File.Open(inputFile, FileMode.Open), Endianness.BigEndian);
            string fileMagic = reader.ReadString(4);
            if (fileMagic != "RWAR")
            {
                Console.WriteLine("Invalid File Magic: " + fileMagic);
                return;
            }

            ushort fileBOM = reader.ReadUInt16();
            if (fileBOM != 0xFEFF)
            {
                Console.WriteLine("Invalid Byte Order Mark: 0x" + fileBOM.ToString("X4"));
                return;
            }

            byte versionMajor = reader.ReadByte(); // 1
            byte versionMinor = reader.ReadByte(); // 0
            int fileLength = reader.ReadInt32();
            ushort headerLength = reader.ReadUInt16();
            ushort sectionCount = reader.ReadUInt16();

            int tablSectionOffset = -1;
            int dataSectionOffset = -1;
            for (int i = 0; i < sectionCount; i++)
            {
                int sectionOffset = reader.ReadInt32();
                int sectionLength = reader.ReadInt32();
                long currentPos = reader.Position;
                reader.Position = sectionOffset;
                string sectionMagic = reader.ReadString(4);
                if (sectionMagic == "TABL")
                    tablSectionOffset = sectionOffset;
                else if (sectionMagic == "DATA")
                    dataSectionOffset = sectionOffset;
                reader.Position = currentPos;
            }

            if (tablSectionOffset < 0)
            {
                Console.WriteLine("TABL section not found");
                return;
            }

            if (dataSectionOffset < 0)
            {
                Console.WriteLine("DATA section not found");
                return;
            }

            reader.Position = tablSectionOffset + 8;
            int nrBrwavs = reader.ReadInt32();

            Directory.CreateDirectory(outputFolder);

            for (int i = 0; i < nrBrwavs; i++)
            {
                uint flags = reader.ReadUInt32(); // 0x01000000
                if (flags != 0x01000000)
                    Console.WriteLine("BRWAV Entry #" + i + " - 0x" + flags.ToString("X8"));

                int brwavOffset = reader.ReadInt32();
                int brwavLength = reader.ReadInt32();
                long currentPos = reader.Position;
                reader.Position = dataSectionOffset + brwavOffset;
                string outputPath = outputFolder + "/" + i + ".brwav";
                File.WriteAllBytes(outputPath, reader.ReadBytes(brwavLength));
                Console.WriteLine("File extracted: " + outputPath);
                reader.Position = currentPos;
            }

            reader.Close();
        }

        static void Create(string inputFolder, string outputFile)
        {
            string[] files = Directory.GetFiles(inputFolder, "*.brwav");
            foreach (string file in files)
            {
                if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out int fileId))
                {
                    Console.WriteLine("Invalid file name: " + Path.GetFileName(file));
                    return;
                }
            }

            List<string> sortedFiles = files
            .Select(file => new
            {
                Path = file,
                Number = int.Parse(Path.GetFileNameWithoutExtension(file))
            })
            .OrderBy(x => x.Number)
            .Select(x => x.Path)
            .ToList();

            EndianWriter writer = new EndianWriter(File.Create(outputFile), Endianness.BigEndian);
            writer.WriteString("RWAR");
            writer.WriteUInt16(0xFEFF);
            writer.WriteByte(1);
            writer.WriteByte(0);
            writer.Position += 4;
            writer.WriteUInt16(0x20);
            writer.WriteUInt16(2);
            writer.WriteInt32(0x20);
            writer.Position = 0x20;
            writer.WriteString("TABL");
            writer.Position += 4;
            writer.WriteInt32(sortedFiles.Count);

            foreach (string file in sortedFiles)
            {
                writer.WriteUInt32(0x01000000);
                writer.Position += 8;
            }

            while (writer.Position % 0x20 != 0)
                writer.WriteByte(0);

            int dataOffset = (int)writer.Position;
            int tablLength = dataOffset - 0x20;
            writer.Position = 0x14;
            writer.WriteInt32(tablLength);
            writer.WriteInt32(dataOffset);
            writer.Position += 8;
            writer.WriteInt32(tablLength);

            writer.Position = dataOffset;
            writer.WriteString("DATA");

            while (writer.Position % 0x20 != 0)
                writer.WriteByte(0);

            long dataStartPos = writer.Position;
            writer.Position = 0x2C;

            int currentBrwavOffset = (int)dataStartPos;
            int i = 0;
            int dataEndOffset = 0;
            foreach (string file in sortedFiles)
            {
                i++;
                writer.Position += 4;
                writer.WriteInt32(currentBrwavOffset - dataOffset);
                long currentPos = writer.Position;
                writer.Position = currentBrwavOffset;
                writer.WriteBytes(File.ReadAllBytes(file));
                int fileLength = (int)writer.Position - currentBrwavOffset;

                while (writer.Position % 4 != 0)
                    writer.WriteByte(0);

                currentBrwavOffset = (int)writer.Position;
                if (i == sortedFiles.Count)
                    dataEndOffset = currentBrwavOffset;

                writer.Position = currentPos;
                writer.WriteInt32(fileLength);
            }

            writer.Position = 8;
            writer.WriteInt32(dataEndOffset);
            writer.Position = 0x1C;
            int dataLength = dataEndOffset - dataOffset;
            writer.WriteInt32(dataLength);
            writer.Position = dataOffset + 4;
            writer.WriteInt32(dataLength);
            writer.Close();
        }
    }
}