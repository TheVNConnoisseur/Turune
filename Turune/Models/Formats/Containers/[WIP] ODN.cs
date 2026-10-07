using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Turune.Models.Formats.Containers;
internal class ODN
{
    internal class ODNScheme
    {
        public List<ODNScheme_Entry> Entries { get; set; } = [];
        public byte[] RawData { get; set; } = [];
        public ODNScheme_Properties Properties { get; set; } = new ODNScheme_Properties();
    }

    internal class ODNScheme_Properties
    {
        public bool IsEncrypted { get; set; } = false;
        public bool EntriesOffsetsAreNotRelativeToRawData { get; set; } = false;
        public bool ContainsAudio { get; set; } = false;
        public string LastEntryOffset { get; set; } = "";
    }

    internal class ODNScheme_Entry
    {
        public string Name = "";
        public uint Offset; //The value is stored as an ASCII

        //The PCM byte length gets ignored since that can be calculated on the fly
    }

    public ODNScheme OdnScheme = new ODNScheme();

    ///<summary>Gets the ODN scheme from an original ODN file.</summary>
    ///<param name="inputFile">Original ODN file.</param>
    public void ParsePacked(byte[] inputFile)
    {
        //A way to know if the container is encrypted or not is by checking if the 1st entry
        //offset contains a valid hexadecimal value.
        string firstEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(8, 8).ToArray());
        if (!uint.TryParse(firstEntryOffset, NumberStyles.HexNumber, null, out _))
        {
            OdnScheme.Properties.IsEncrypted = true;
            for (int i = 0; i < inputFile.Length; i++)
                inputFile[i] = (byte)((0xFF - i) ^ inputFile[i]);
            
            //Recalculate the first entry offset after decryption
            firstEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(8, 8).ToArray());
        }

        //Check if the entries offsets are relative to the raw data or not.
        if (firstEntryOffset != "00000000")
            OdnScheme.Properties.EntriesOffsetsAreNotRelativeToRawData = true;

        //To know if the container contains audio, we need to check if what follows is the
        //PCM byte length of the first entry. Sadly, since the game does not care about
        //file names, a good way to confirm if it contains audio files is by checking
        //if it contains the Vorbis encoder string, which is long enough to not be present
        //as a file name and unique enough to not be present in a non-audio file.
        OdnScheme.Properties.ContainsAudio = inputFile.AsSpan().IndexOf("aoTuV b5d [20090301] (based on Xiph.Org's libVorbis)"u8) >= 0;

        int currentOffset = 0;

        //Parse the index of entries. The first case need to have their offsets normalized to 0.
        if (OdnScheme.Properties.EntriesOffsetsAreNotRelativeToRawData)
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
                if (OdnScheme.Properties.ContainsAudio)
                    currentOffset += 8; //Skip the PCM byte length of the entry
                currentOffset += 16;
            }

            if (OdnScheme.Properties.ContainsAudio)
                OdnScheme.Entries.Remove(OdnScheme.Entries[^1]); //For audio files, the last entry has no data associated with it

            //Copy the contents from all entries
            OdnScheme.RawData = new byte[inputFile.Length - rawDataOffset];
            Buffer.BlockCopy(inputFile, (int)rawDataOffset, OdnScheme.RawData, 0, OdnScheme.RawData.Length);
        }

        //The second case does not need to have their offsets normalized to 0.
        else
        {
            while (currentOffset < inputFile.Length)
            {
                //To know when the index ends, we need to find the entry with the offset "ffffffff" in ASCII
                string currentEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset + 8, 8).ToArray());
                if (currentEntryOffset == "ffffffff")
                {
                    OdnScheme.Properties.LastEntryOffset = currentEntryOffset;
                    currentOffset += 16;    //Skip the bytes that conform the index delimiter entry
                    break;
                }

                ODNScheme_Entry entry = new ODNScheme_Entry
                {
                    Name = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 8).ToArray()),
                    Offset = uint.Parse(currentEntryOffset, NumberStyles.HexNumber)
                };

                OdnScheme.Entries.Add(entry);
                currentOffset += 16;
            }

            //Copy the contents from all entries
            OdnScheme.RawData = new byte[inputFile.Length - currentOffset];
            Buffer.BlockCopy(inputFile, currentOffset, OdnScheme.RawData, 0, OdnScheme.RawData.Length);
        }
    }
}