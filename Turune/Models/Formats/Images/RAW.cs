using System;
using System.Collections.Generic;
using System.IO;

namespace Turune.Models.Formats.Images;

internal class RAW
{
    internal class RAWScheme
    {
        public List<byte> Data { get; set; } = [];
        public RAWScheme_Properties Properties { get; set;} = new RAWScheme_Properties();
    }
    internal class RAWScheme_Properties
    {
        public bool HasAlphaLayer;
        public Resolution Resolution;
    }

    public RAWScheme RawScheme { get; private set; } = new RAWScheme();

    //The game does not store the resolution of the images inside them, instead, those gets calculated on the fly based
    //on the game's width and height resolution. A notable exception to that are the images used for the character names
    //inside the message box, because those get their size set elsewhere.
    internal readonly record struct Resolution(int Width, int Height)
    {
        //The first combination of values are for the images that occupy the entire screen
        public static readonly Resolution FullScreen_Small = new(640, 480);
        public static readonly Resolution FullScreen_Medium = new(800, 600);
        public static readonly Resolution FullScreen_High = new(1024, 768);

        //The game's script system has a function called SetNameSystem, which two of its parameters are the width and height sizes
        //of each of the images inside the data08.odn file. This means that while there's no stopping anybody from changing
        //the size of these images, in reality, given the nature of the engine, it is better to not bother changing the size
        //of these images
        public static readonly Resolution CharacterName = new(400, 200);

        public static IEnumerable<Resolution> FullScreen { get; } = [FullScreen_Small, FullScreen_Medium, FullScreen_High];
        public static IEnumerable<Resolution> NonFullScreen { get; } = [CharacterName];
    }

    public void ParsePacked(byte[] inputFile)
    {
        //The first thing to do is try to inpher the resolution of the image, which will let us also know its BPP
        foreach (var resolution in Resolution.FullScreen)
        {   
            int currentOffset = 0;  //Reset the pointer to 0 to check every possible resolution
            RawScheme.Data.Clear();
            int expectedFileSize = 3 * resolution.Width * resolution.Height;
            while (RawScheme.Data.Count < expectedFileSize)
            {
                try
                {
                    RLE_Decode(ref inputFile, ref currentOffset, false, expectedFileSize);
                }
                catch(IndexOutOfRangeException)
                {
                    break;
                }
            }

            //To know if the selected resolution is the correct one, the file size of the image should coincide with the one calculated previously (or at least, slightly bigger)
            if (expectedFileSize == RawScheme.Data.Count)
            {
                RawScheme.Properties.Resolution = resolution;
                RawScheme.Properties.HasAlphaLayer = false;
                return;
            }
        }

        foreach (var resolution in Resolution.NonFullScreen)
        {
            int currentOffset = 0;  //Reset the pointer to 0 to check every possible resolution
            RawScheme.Data.Clear();
            int expectedFileSize = 4 * resolution.Width * resolution.Height;
            while (RawScheme.Data.Count < expectedFileSize)
            {
                try
                {
                    RLE_Decode(ref inputFile, ref currentOffset, true, expectedFileSize);
                }
                catch(IndexOutOfRangeException)
                {
                    break;
                }
            }

            //To know if the selected resolution is the correct one, the file size of the image should coincide with the one calculated previously (or at least, slightly bigger)
            if (expectedFileSize == RawScheme.Data.Count)
            {
                RawScheme.Properties.Resolution = resolution;
                RawScheme.Properties.HasAlphaLayer = true;
                return;
            }
        }
    
        throw new InvalidDataException("The loaded image does not have a valid resolution.");
    }

    /// <summary>
    /// Function that is in charge of decoding the image encoded using the run-length encoding.
    /// </summary>
    /// <param name="encodedArray">The array of the encoded image.</param>
    /// <param name="currentOffset">The offset of the currently analyzed byte.</param>
    /// <param name="hasAlphaLayer">If the analyzed image supposedly has an alpha layer or not.</param>
    /// <param name="maxSizeFile">The maximum size in bytes that the file can occupy.</param>
    public void RLE_Decode(ref byte[] encodedArray, ref int currentOffset, bool hasAlphaLayer, int maxSizeFile)
    {
        byte rle_Control = encodedArray[currentOffset++]; //This byte determines for the next 8 pixels if they get repeated or not
        for (int bit_index = 0; bit_index < 8; bit_index++)
        {
            byte pixel_B = encodedArray[currentOffset++];
            byte pixel_G = encodedArray[currentOffset++];
            byte pixel_R = encodedArray[currentOffset++];
            byte pixel_A = 0;
            if (hasAlphaLayer)
                pixel_A = encodedArray[currentOffset++];

            //Get the bit that corresponding to the current pixel, and if it's a 1, it means that the pixel gets repeated
            bool rle_HasRun = ((rle_Control >> bit_index) & 1) != 0;
            int rle_Length = 0;
            if (rle_HasRun)
                rle_Length = (encodedArray[currentOffset++] << 8) + encodedArray[currentOffset++]; //The RLE length is in big-endian 16-bit
            
            //Write the pixel to the output as many times as the rle_Length variable specifies. Note that the pixel will be drawn at least 1 time no matter what.
            for (int i = 0; i <= rle_Length; i++)
            {
                if (hasAlphaLayer)
                    RawScheme.Data.AddRange(pixel_B, pixel_G, pixel_R, pixel_A);
                else
                    RawScheme.Data.AddRange(pixel_B, pixel_G, pixel_R);

                if (RawScheme.Data.Count >= maxSizeFile) //Ensure that the image does not exceed the expected file size bounds
                    return;
            }
        }
    }
}