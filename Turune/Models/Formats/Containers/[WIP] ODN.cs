using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Turune.Models.Formats.Containers;
internal class ODN
{
    internal class ODNScheme
    {
        public List<ODNScheme_Entry> Entries { get; set; } = [];
        public ODNScheme_Properties Properties { get; set; } = new ODNScheme_Properties();
    }
    internal class ODNScheme_Entry
    {
        public string Name = "";
        public byte[] Data = [];

        //The PCM byte length gets ignored since that can be calculated on the fly
    }
    internal class ODNScheme_Properties
    {
        public bool IsEncrypted { get; set; } = false;
        public bool EntriesOffsetsAreNotRelativeToRawData { get; set; } = false;
        public bool ContainsAudio { get; set; } = false;
        public string LastEntryOffset { get; set; } = "";
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

        //Obtain the offset where the raw data for all entries actually start
        int rawDataOffset = 0;
        if (OdnScheme.Properties.EntriesOffsetsAreNotRelativeToRawData)
            rawDataOffset = int.Parse(firstEntryOffset, NumberStyles.HexNumber);
        else
        {
            while (rawDataOffset < inputFile.Length)
            {
                //To know when the index ends, we need to find the entry with the offset "ffffffff" in ASCII
                string currentEntryOffset = Encoding.ASCII.GetString(inputFile.AsSpan(rawDataOffset + 8, 8).ToArray());
                if (currentEntryOffset == "ffffffff")
                {
                    OdnScheme.Properties.LastEntryOffset = currentEntryOffset;
                    if (OdnScheme.Properties.ContainsAudio)
                        rawDataOffset += 24;
                    else
                        rawDataOffset += 16;
                    break;
                }
                //Jump to the next entry
                if (OdnScheme.Properties.ContainsAudio)
                    rawDataOffset += 24;
                else
                    rawDataOffset += 16;
            }
        }
        
        int currentOffset = 0;
        //Obtain the data and names of all entries
        while (currentOffset < rawDataOffset)
        {
            string currentEntryOffsetASCII = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset + 8, 8).ToArray());

            //If the current entry has an index delimiter, it means that
            //we have reached the end of the index
            if (currentEntryOffsetASCII == OdnScheme.Properties.LastEntryOffset)
                break;
            
            int currentEntryOffset;
            if (OdnScheme.Properties.EntriesOffsetsAreNotRelativeToRawData)
                currentEntryOffset = int.Parse(currentEntryOffsetASCII, NumberStyles.HexNumber) - rawDataOffset;
            else
                currentEntryOffset = int.Parse(currentEntryOffsetASCII, NumberStyles.HexNumber);
            
            //Calculate the next entry's offset in order to get the current entry's size
            string nextEntryOffsetASCII;
            int nextEntryOffset;
            if (OdnScheme.Properties.ContainsAudio)
                nextEntryOffsetASCII = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset + 8 + 24, 8).ToArray());
            else
                nextEntryOffsetASCII = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset + 8 + 16, 8).ToArray());
            
            if (OdnScheme.Properties.EntriesOffsetsAreNotRelativeToRawData)
            {
                //The last entry does not have any other entry to check its size, and when parsing its value
                //will always throw a FormatException because it's reading the bytes from the first entry.
                try
                {
                    nextEntryOffset = int.Parse(nextEntryOffsetASCII, NumberStyles.HexNumber) - rawDataOffset;
                }
                catch (FormatException)
                {
                    nextEntryOffset = inputFile.Length;
                }
            }
            else
            {
                //The last entry does not have any other entry to check its size, but since the index
                //ends with a hardcoded offset, a simple check facilitates the job.
                if (nextEntryOffsetASCII != OdnScheme.Properties.LastEntryOffset)
                    nextEntryOffset = int.Parse(nextEntryOffsetASCII, NumberStyles.HexNumber);
                else
                    nextEntryOffset = inputFile.Length;
            }
            
            ODNScheme_Entry entry = new ODNScheme_Entry
            {
                Name = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 8).ToArray()),
                Data = inputFile.AsSpan(currentEntryOffset, nextEntryOffset - currentEntryOffset).ToArray()
            };
            OdnScheme.Entries.Add(entry);

            //Move the offset to the next entry
            if (OdnScheme.Properties.ContainsAudio)
                currentOffset += 24;
            else
                currentOffset += 16;
        }

        //For audio files, the last entry has no data associated with it
        if (OdnScheme.Properties.ContainsAudio)
            OdnScheme.Entries.Remove(OdnScheme.Entries[^1]);
    }

    ///<summary>Exports to a byte array each of the entries inside the ODNScheme.
    /// Each entry gets its own respective byte array</summary>
    public byte[][] Export()
    {
        byte[][] entryData = new byte[OdnScheme.Entries.Count][];
        foreach (var entry in OdnScheme.Entries)
        {
            
        }
        return null;
    }
}