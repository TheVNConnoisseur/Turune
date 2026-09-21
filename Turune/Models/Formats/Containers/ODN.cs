using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Turune.Models.Formats.Initialization;

namespace Turune.Models.Formats.Containers
{
    internal class ODN
    {
        internal class ODNScheme
        {
            public List<ODNScheme_Entry> Entries { get; set; } = [];
            public byte[] RawData { get; set; } = [];
        }

        internal class ODNScheme_Entry
        {
            public string Name = "";
            public uint Offset; //The value is stored as an ASCII
        }

        public ODNScheme OdnScheme = new ODNScheme();

        ///<summary>Gets the INI scheme from an original system.dat file.</summary>
        ///<param name="inputFile">Original ODN file.</param>
        public void ParsePacked(byte[] inputFile)
        {
            //Some ODN files are encrypted, so first it's mandatory to check if this one it is.
            //An easy way to confirm that, is to check the 1st entry offset, and see if we get
            //a valid hexadecimal value. If that's not the case, we can conclude that we have
            //an encrypted ODN file.
            string firstEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(8, 8).ToArray());
            if (!uint.TryParse(firstEntryOffset, NumberStyles.HexNumber, null, out uint offset))
            {
                for (int i = 0; i < inputFile.Length; i++)
                {
                    inputFile[i] = (byte)((0xFF - i) ^ inputFile[i]);
                }
            }

            firstEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(8, 8).ToArray());  //Recalculate the offset in case the file was encrypted
            int currentOffset = 0;
            if (firstEntryOffset != "00000000")
            {
                uint rawDataOffset = uint.Parse(firstEntryOffset, NumberStyles.HexNumber);
                while (currentOffset < rawDataOffset)
                {
                    string currentEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset + 8, 8).ToArray());

                    ODNScheme_Entry entry = new ODNScheme_Entry
                    {
                        Name = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 8).ToArray()),
                        Offset = uint.Parse(currentEntryOffset, NumberStyles.HexNumber) - rawDataOffset //Normalize to 0
                    };

                    OdnScheme.Entries.Add(entry);
                    currentOffset += 16;
                }

                //Copy the contents from all entries
                OdnScheme.RawData = new byte[inputFile.Length - rawDataOffset];
                Buffer.BlockCopy(inputFile, (int)rawDataOffset, OdnScheme.RawData, 0, OdnScheme.RawData.Length);
            }
            else
            {
            }
        }
    }
}
