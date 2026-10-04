using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Turune.Models.Formats.Initialization;

internal class INI
{
    internal class INIScheme
    {
        public string CatalogNumber { get; set; } = "";
        public string WindowTitle { get; set; } = "";
        public byte GameResolution { get; set; }
        public byte VoiceChannels { get; set; }
        public string[] CharacterNames { get; set; } = new string[3];
    }

    public INIScheme IniScheme { get; private set; } = new INIScheme();

    ///<summary>Gets the INI scheme from an original system.ini file.</summary>
    ///<param name="inputFile">Original system.ini file.</param>
    public void ParsePacked(byte[] inputFile)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding shiftJIS = Encoding.GetEncoding("shift-jis");

        int currentOffset = 0;

        //Catalog number (or executable name, to be more precise)
        uint lengthCatalogNumber = Convert.ToUInt32(Encoding.ASCII.GetString(inputFile.AsSpan(0x0, 4)));
        currentOffset += 4;

        
        byte[] encryptedCatalogNumber = inputFile.AsSpan(currentOffset, (int)lengthCatalogNumber).ToArray();
        for (int i = 0; i < encryptedCatalogNumber.Length; i++)
            encryptedCatalogNumber[i] ^= 0xD1;
        currentOffset += (int)lengthCatalogNumber;

        IniScheme.CatalogNumber = shiftJIS.GetString(encryptedCatalogNumber);

        //Window title
        uint windowTitleLength = Convert.ToUInt32(Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 4)));
        currentOffset += 4;

        byte[] encryptedWindowTitle = inputFile.AsSpan(currentOffset, (int)windowTitleLength).ToArray();
        for (int i = 0; i < encryptedWindowTitle.Length; i++)
            encryptedWindowTitle[i] ^= 0xD1;
        currentOffset += (int)windowTitleLength;

        IniScheme.WindowTitle = shiftJIS.GetString(encryptedWindowTitle);

        //Game resolution
        IniScheme.GameResolution = inputFile[currentOffset];
        currentOffset++;

        //Voice channels
        IniScheme.VoiceChannels = Convert.ToByte(Encoding.ASCII.GetString(inputFile.AsSpan(currentOffset, 1)));
        currentOffset++;

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
            IniScheme.CharacterNames[i] = shiftJIS.GetString(characterNamesArray[i].ToArray()).Replace("\0", "");
    }

    ///<summary>Gets the INI scheme from a normal INI file.</summary>
    ///<param name="inputLines">INI file as an array of strings.</param>
    public void ParseUnpacked(string[] inputLines)
    {
        var iniReader = new Utils.IniFile();
        iniReader.Parse(inputLines);

        //Sanity check to ensure that the game has the fundamental sections
        string[] requiredSections =
        {
            "General", "Debug"
        };

        string[] missingSections = requiredSections.Where(section => !iniReader.SectionExists(section)).ToArray();

        if (missingSections.Length > 0)
            throw new Exception($"The INI file is missing the following sections: {string.Join(", ", missingSections)}");

        IniScheme.CatalogNumber = iniReader.Read("General", "CatalogNumber");
        IniScheme.WindowTitle = iniReader.Read("General", "WindowTitle");
        IniScheme.GameResolution = byte.Parse(iniReader.Read("General", "Resolution"));
        IniScheme.VoiceChannels = byte.Parse(iniReader.Read("Debug", "VoiceChannels"));
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

        //The byte count has to be done in shiftJIS because the title
        //is in Shift-JIS, so it takes into consideration the double-byte
        //characters.
        string lengthCatalogNumber = shiftJIS.GetByteCount(IniScheme.CatalogNumber).ToString("D4"); 
        outputFile.AddRange(Encoding.ASCII.GetBytes(lengthCatalogNumber));

        byte[] encryptedCatalogNumber = shiftJIS.GetBytes(IniScheme.CatalogNumber);
        for (int i = 0; i < encryptedCatalogNumber.Length; i++)
            encryptedCatalogNumber[i] ^= 0xD1;
        outputFile.AddRange(encryptedCatalogNumber);

        string lengthWindowTitle = shiftJIS.GetByteCount(IniScheme.WindowTitle).ToString("D4");
        outputFile.AddRange(Encoding.ASCII.GetBytes(lengthWindowTitle));

        byte[] encryptedWindowTitle = shiftJIS.GetBytes(IniScheme.WindowTitle);
        for (int i = 0; i < encryptedWindowTitle.Length; i++)
            encryptedWindowTitle[i] ^= 0xD1;
        outputFile.AddRange(encryptedWindowTitle);

        byte gameResolution = IniScheme.GameResolution;
        outputFile.Add(gameResolution);

        byte voiceChannels = Encoding.ASCII.GetBytes("0")[0]; //The game never really bothers with this value, so it is always set to 0.
        outputFile.Add(voiceChannels);

        byte[] characterNames = new byte[144]; //Hardcoded value from what it seems, all of the games have this length for the character names.
        characterNames.Initialize(); //To ensure that the array is filled with 0s.

        for (int i = 0; i < 3; i++)
        {
            byte[] characterName = shiftJIS.GetBytes(IniScheme.CharacterNames[i]);
            for (int j = 0; j < characterName.Length; j++)
                characterNames[i + 3 * j] = characterName[j];           //Character names are interleaved in the original system.ini file.
        }
        outputFile.AddRange(characterNames);

        return outputFile.ToArray();
    }
}