using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NVorbis;

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
        public int? PCMByteLength;
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
        //PCM byte length of the first entry. Sadly, since the game does not store file
        //extensions, a good way to confirm if it contains audio files is by checking
        //if it contains certain strings typically found in OGG Vorbis files.
        OdnScheme.Properties.ContainsAudio = 
            (inputFile.AsSpan().IndexOf("libVorbis"u8) >= 0 ||
            inputFile.AsSpan().IndexOf("libvorbis"u8) >= 0) &&
            inputFile.AsSpan().IndexOf("OggS"u8) >= 0;

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
                currentEntryOffset = int.Parse(currentEntryOffsetASCII, NumberStyles.HexNumber);
            else
                currentEntryOffset = int.Parse(currentEntryOffsetASCII, NumberStyles.HexNumber) + rawDataOffset;
            
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
                    nextEntryOffset = int.Parse(nextEntryOffsetASCII, NumberStyles.HexNumber);
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
                    nextEntryOffset = int.Parse(nextEntryOffsetASCII, NumberStyles.HexNumber) + rawDataOffset;
                else
                    nextEntryOffset = inputFile.Length;
            }
            
            //Since audio files are always OGG Vorbis files, we can put their respective extension to it
            if (OdnScheme.Properties.ContainsAudio)
            {
                ODNScheme_Entry entry = new ODNScheme_Entry
                {
                    Name = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 8).ToArray()) + ".ogg",
                    Data = inputFile.AsSpan(currentEntryOffset, nextEntryOffset - currentEntryOffset).ToArray()
                };
                OdnScheme.Entries.Add(entry);
            }
            else
            {
                ODNScheme_Entry entry = new ODNScheme_Entry
                {
                    Name = Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 8).ToArray()),
                    Data = inputFile.AsSpan(currentEntryOffset, nextEntryOffset - currentEntryOffset).ToArray()
                };
                OdnScheme.Entries.Add(entry);
            }

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

    ///<summary>Generates the ODNScheme from an arbitrary amount of input files.</summary>
    ///<param name="inputNames">Names of the files to compress.</param>
    ///<param name="inputData">Byte arrays of the files to compress.</param>
    public void ParseUnpacked(string[] inputNames, byte[][] inputData)
    {
        //Check their names to see if they are named correctly
        int numberAudioFiles = 0;
        foreach (var entryName in inputNames)
        {
            //Ensure that they have the appropiate file name length.
            if (Path.GetFileNameWithoutExtension(entryName).Length != 8)
                throw new InvalidDataException("File " + entryName + " is not 8 characters long.");
            
            //Check if all or none of the files are audio ones, and throw an
            //error if only some of them are.
            if (Path.GetExtension(entryName) == ".ogg")
                numberAudioFiles++;
        }

        //Determine if the ODN file will contain audio or not based on the input entries
        if (numberAudioFiles == 0)
            OdnScheme.Properties.ContainsAudio = false;
        else if (numberAudioFiles == inputNames.Length)
            OdnScheme.Properties.ContainsAudio = true;
        else
            throw new InvalidDataException("Some files contain audio, and ODN files can either have only OGG files or none at all.");

        //Ensure that no file (neither the sum of them) exceeds the file size limit imposed by the length of an entry's offset
        uint totalEntriesSize = 0;
        foreach (var entryData in inputData)
        {
            //Ensure that no file exceeds the maximum size the offset has
            if ((uint)entryData.Length >= uint.MaxValue)
                throw new InvalidDataException("No file cannot have a size equal or greater than 4294967295 (or 0xFFFFFFFF) bytes, and one of " +
                 "the files exceeds this limit.");
            
            totalEntriesSize += (uint)entryData.Length;
            if (totalEntriesSize >= uint.MaxValue)
                throw new InvalidDataException("All of the selected files have a size equal or greater than 4294967295 (or 0xFFFFFFFF) bytes, please " +
                "ensure that the selected files do not exceed this limit.");
        }

        //Create all of the ODNScheme_Entry required to host all of the input files
        for (int i = 0; i < inputNames.Length; i++)
        {
            if (OdnScheme.Properties.ContainsAudio)
            {
                using var vorbisStream = new MemoryStream(inputData[i]);
                using var vorbis = new VorbisReader(vorbisStream);
                
                //The decoder only accepts mono and 44.1KHz audio files
                if (vorbis.Channels != 1 && vorbis.SampleRate != 44100)
                    throw new InvalidDataException("The file " + inputNames[i] + "is either not mono or does not have a sample rate of 44.1KHz.");

                var entry = new ODNScheme_Entry
                {
                    Name = Path.GetFileNameWithoutExtension(inputNames[i]),
                    Data = inputData[i],
                    PCMByteLength = (int)(vorbis.TotalSamples * vorbis.Channels * 2) //8-bit * 2 because the audio files are 16-bit
                };
                OdnScheme.Entries.Add(entry);
            }
            else
            {
                var entry = new ODNScheme_Entry
                {
                    Name = Path.GetFileNameWithoutExtension(inputNames[i]),
                    Data = inputData[i]
                };
                OdnScheme.Entries.Add(entry);
            }
        }
    }
}