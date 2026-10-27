using System;
using System.Collections.Generic;
using System.Drawing;
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
    }

    public void ParsePacked(byte[] inputFile)
    {
        //The first thing to do is try to inpher the resolution of the image, which will let us also know its BPP
        foreach (var resolution in Resolution.FullScreen)
        {   
            int currentOffset = 0;  //Reset the pointer to 0 to check every possible resolution
            while(currentOffset < inputFile.Length)
            {
                try
                {
                    byte rle_Control = inputFile[currentOffset++]; //This byte determines for the next 8 pixels if they get repeated or not
                    for (int bit_index = 0; bit_index < 8; bit_index++)
                    {
                        byte pixel_B = inputFile[currentOffset++];
                        byte pixel_G = inputFile[currentOffset++];
                        byte pixel_R = inputFile[currentOffset++];
                        
                        //Get the bit that corresponding to the current pixel, and if it's a 1, it means that the pixel gets repeated
                        bool rle_HasRun = ((rle_Control >> bit_index) & 1) != 0;
                        int rle_Length = 0;
                        if (rle_HasRun)
                            rle_Length = (inputFile[currentOffset++] << 8) + inputFile[currentOffset++]; //The RLE length is in big-endian 16-bit
                        
                        //Write the pixel to the output as many times as the rle_Length variable specifies. Note that the pixel will be drawn at least 1 time no matter what.
                        for (int i = 0; i <= rle_Length; i++)
                            RawScheme.Data.AddRange(pixel_B, pixel_G, pixel_R);
                    }
                }
                catch(IndexOutOfRangeException)
                {
                    //Jump to the next possible resolution
                }
            }
        }
    }
}