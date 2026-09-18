using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Turune.Models.Formats.Initialization
{
    internal class INI
    {
        internal class INIScheme
        {
            public string CatalogNumber { get; set; } = "";
            public string WindowTitle { get; set; } = "";
            public byte GameResolution { get; set; }
            public byte VoiceChannels { get; set; }
            public string[] CharacterNames { get; set; } = Array.Empty<string>();
        }

        public INIScheme IniScheme { get; private set; }

        ///<summary>Gets the INI scheme from an original system.ini file.</summary>
        public void ParsePacked(byte[] inputFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            int currentOffset = 0;

            //Catalog number (or executable name, to be more precise)
            uint lengthCatalogNumber = Convert.ToUInt32(Encoding.ASCII.GetString(inputFile.AsSpan(0x0, 4)));
            currentOffset += 4;

            
            byte[] encryptedCatalogNumber = inputFile.AsSpan(0x4, (int)lengthCatalogNumber).ToArray();
            for (int i = 0; i < encryptedCatalogNumber.Length; i++)
                encryptedCatalogNumber[i] ^= 0xD1;
            currentOffset += (int)lengthCatalogNumber;

            string catalogNumber = shiftJIS.GetString(encryptedCatalogNumber);

            //Window title
            uint windowTitleLength = Convert.ToUInt32(Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 4)));
            currentOffset += 4;

            byte[] encryptedWindowTitle = inputFile.AsSpan(currentOffset, (int)windowTitleLength).ToArray();
            for (int i = 0; i < encryptedWindowTitle.Length; i++)
                encryptedWindowTitle[i] ^= 0xD1;
            currentOffset += (int)windowTitleLength;

            string windowTitle = shiftJIS.GetString(encryptedWindowTitle);

            //Game resolution
            byte gameResolution = inputFile[currentOffset];
            currentOffset++;

            //Voice channels
            byte voiceChannels = Convert.ToByte(Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 1)));
            currentOffset++;

            string[] characterNames = new string[3];
            List<List<byte>> characterNamesArray = new List<List<byte>>();

            for (int i = 0; i < 3; i++)
                characterNamesArray.Add(new List<byte>());

            //Since it is a debug function, the game doesn't really bother with this
            //value being to 0. It always assumes that there are 3 names to parse.
            while (currentOffset < inputFile.Length)
            {
                for (int i = 0; i < 3; i++)
                {
                    characterNamesArray[i].Add(inputFile[currentOffset]);
                    currentOffset++;
                }
            }

            for (int i = 0; i < 3; i++)
                characterNames[i] = shiftJIS.GetString(characterNamesArray[i].ToArray()).Replace("\0", "");

            IniScheme = new INIScheme
            {
                CatalogNumber = catalogNumber,
                WindowTitle = windowTitle,
                GameResolution = gameResolution,
                VoiceChannels = voiceChannels,
                CharacterNames = characterNames
            };
        }

        ///<summary>Gets the INI scheme from a normal INI file.</summary>
        public void ParseUnpacked()
        {
            var iniReader = new Utils.IniFile();
            if (!iniReader.SectionExists("General") || !iniReader.SectionExists("Debug"))
                throw new Exception("The INI file is missing required sections.");

            IniScheme = new INIScheme
            {
                CatalogNumber = iniReader.Read("General", "CatalogNumber"),
                WindowTitle = iniReader.Read("General", "WindowTitle"),
                GameResolution = byte.Parse(iniReader.Read("General", "Resolution")),
                VoiceChannels = byte.Parse(iniReader.Read("Debug", "VoiceChannels")),
                CharacterNames = new string[3]
            };

            for (int i = 0; i < 3; i++)
                IniScheme.CharacterNames[i] = iniReader.Read("Debug", $"Character_{i + 1}");
        }

        ///<summary>Exports the INI scheme to a string array.</summary>
        public string[] Export()
        {
            var iniCreator = new Utils.IniFile();

            iniCreator.Write("General", "CatalogNumber", IniScheme.CatalogNumber);
            iniCreator.Write("General", "WindowTitle", IniScheme.WindowTitle);
            iniCreator.Write("General", "Resolution", IniScheme.GameResolution.ToString());

            iniCreator.Write("Debug", "VoiceChannels", IniScheme.VoiceChannels.ToString());
            for (int i = 0; i < IniScheme.CharacterNames.Length; i++)
                iniCreator.Write("Debug", $"Character_{i + 1}", IniScheme.CharacterNames[i]);

            return iniCreator.Create();
        }

        ///<summary>Create an original system.ini file from the INI scheme.</summary>
        public byte[] Generate()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

            List<byte> outputFile = new List<byte>();

            byte[] lengthCatalogNumber = Encoding.ASCII.GetBytes(
                Encoding.ASCII.GetByteCount(IniScheme.CatalogNumber).ToString("D4"));


            return null;
        }
    }
}
