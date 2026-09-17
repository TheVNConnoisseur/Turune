using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Turune.Models.Formats.Initialization
{
    internal class INI
    {
        public static void Parse(byte[] inputFile)
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

            string catalogNumber = Encoding.ASCII.GetString(encryptedCatalogNumber);

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
        }
    }
}
