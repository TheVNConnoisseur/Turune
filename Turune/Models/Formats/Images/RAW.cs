using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Turune.Models.Utils;

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

    /// <summary>
    /// Validates a given RAW image, obtaining its resolution, bit-depth and its decoded bitmap data.
    /// </summary>
    /// <param name="inputFile">Array of bytes that contain the entire image data.</param>
    /// <exception cref="InvalidDataException">The image does not have any of the resolutions the MICo engine expects.</exception>
    public void ParseRAW(byte[] inputFile)
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

            //To know if the selected resolution is the correct one, there are some checks that have to be done, some of them are just
            //simple edge cases that are a consequence of this process being completely heuristic
            if (expectedFileSize == RawScheme.Data.Count && ((currentOffset == inputFile.Length + 1) || (currentOffset == inputFile.Length - 1)))
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
    /// Generates a BMP file of the loaded RAW image.
    /// </summary>
    /// <returns>Byte array of the RAW image in BMP format uncompressed.</returns>
    public byte[] Export()
    {
        //8 bits = 1 byte, so 3 colours + 1 alpha layer =  8 * 4 = 32 bits
        if (RawScheme.Properties.HasAlphaLayer)
            return Utils.ImageFile.BMP.Write(
                RawScheme.Data.ToArray(), new PixelFormat.BGRA32.BMP(), RawScheme.Properties.Resolution.Width, RawScheme.Properties.Resolution.Height);
        //8 bits = 1 byte, so 3 colours =  8 * 3 = 24 bits
        else
            return Utils.ImageFile.BMP.Write(
                RawScheme.Data.ToArray(), new PixelFormat.BGR24.BMP(), RawScheme.Properties.Resolution.Width, RawScheme.Properties.Resolution.Height);
    }

    /// <summary>
    /// Validates a given Bitmap, obtaining its resolution and bit-depth.
    /// </summary>
    /// <param name="inputFile">Bitmap of the file to validate.</param>
    public void ParseBitmap(byte[] inputFile)
    {
        var bmp = ImageFile.BMP.Read(inputFile);
        int bmp_Width = 0;
        int bmp_Height = 0;
        ushort bmp_BitCount = 0;

        if (bmp.CoreHeader is not null)
        {
            bmp_Width = bmp.CoreHeader.Value.bcWidth;
            bmp_Height = bmp.CoreHeader.Value.bcHeight;
            bmp_BitCount = bmp.CoreHeader.Value.bcBitCount;
        }
        else if (bmp.InfoHeader is not null)
        {
            bmp_Width = bmp.InfoHeader.Value.biWidth;
            bmp_Height = bmp.InfoHeader.Value.biHeight;
            bmp_BitCount = bmp.InfoHeader.Value.biBitCount;
        }
        else if (bmp.V4Header is not null)
        {
            bmp_Width = bmp.V4Header.Value.bV4Width;
            bmp_Height = bmp.V4Header.Value.bV4Height;
            bmp_BitCount = bmp.V4Header.Value.bV4BitCount;
        }
        else if (bmp.V5Header is not null)
        {
            bmp_Width = bmp.V5Header.Value.bV5Width;
            bmp_Height = bmp.V5Header.Value.bV5Height;
            bmp_BitCount = bmp.V5Header.Value.bV5BitCount;
        }
        
        //First check if the image has a valid resolution
        foreach (var resolution in Resolution.FullScreen)
        {
            if (bmp_Width != resolution.Width || bmp_Height != resolution.Height)
                continue;
            else
            {
                RawScheme.Properties.Resolution = resolution;
                RawScheme.Properties.HasAlphaLayer = false;
            }
        }

        foreach (var resolution in Resolution.NonFullScreen)
        {
            if (bmp_Width != resolution.Width || bmp_Height != resolution.Height)
                continue;
            else
            {
                RawScheme.Properties.Resolution = resolution;
                RawScheme.Properties.HasAlphaLayer = true;
            }
        }

        if (RawScheme.Properties.Resolution.Width == default)
            throw new InvalidDataException("The loaded image does not have a valid resolution.");
        
        //Check if the image has a valid bit-depth (ideally it would check the pixel format, but the pixel formats the game
        //uses are not available in SkiaSharp)
        if ((RawScheme.Properties.HasAlphaLayer && bmp_BitCount != 32)
            || (!RawScheme.Properties.HasAlphaLayer && bmp_BitCount != 24))
            throw new InvalidDataException("The loaded image does not have a valid bit-depth (pixel format).");

        RawScheme.Data = bmp.PixelData.ToList();
    }
    
    public byte[] Import()
    {
        List<byte> outputImage = [];
        int currentOffset = 0;

        while (currentOffset < RawScheme.Data.Count)
            outputImage.AddRange(RLE_Encode(ref currentOffset));

        return outputImage.ToArray();
    }
    
    /// <summary>
    /// Function that is in charge of decoding the image encoded using the run-length encoding.
    /// It only does 1 run instead of reading the entire file.
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

            //Get the bit that corresponds to the current pixel, and if it's a 1, it means that the pixel gets repeated
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
                    break;
            }
        }
    }

    public byte[] RLE_Encode(ref int currentOffset)
    {
        List<byte> encodedArray = [];
        int[] rle_Length =  new int[8];  //Store in an array how many times all pixels do repeat
        byte[][] pixelsToCopy = new byte[8][];  //Store the pixels themselves that have to be copied

        for (int pixel_index = 0; pixel_index < 8; pixel_index++)
        {
            //Get the current pixel to analyze and see if it repeats or not
            byte[] currentPixel = [];
            byte[] nextPixel = [];
            try
            {
                currentPixel = ReadPixel(ref currentOffset);
                pixelsToCopy[pixel_index] = currentPixel;
            }
            catch (ArgumentOutOfRangeException)
            {
                break;  //No more pixels to copy
            }
            try
            {
                nextPixel = ReadPixel(ref currentOffset);
            }
            catch (ArgumentOutOfRangeException)
            {
                pixelsToCopy[pixel_index] = currentPixel;
                break;
            }

            //Now check how many times the current pixel does get repeated
            int rle_Length_CurrentPixel = 0;
            
            while (currentPixel.AsSpan().SequenceEqual(nextPixel))
            {
                currentPixel = nextPixel;
                try
                {
                    nextPixel = ReadPixel(ref currentOffset);
                }
                catch(ArgumentOutOfRangeException)
                {
                    rle_Length[pixel_index] = rle_Length_CurrentPixel;
                    break;
                }
                rle_Length_CurrentPixel++;
            }

            rle_Length[pixel_index] = rle_Length_CurrentPixel;

            //Due to the offset being advanced by getting the next pixel for the comparison,
            //to ensure that no input pixel gets dropped, we fix the offset
            if (RawScheme.Properties.HasAlphaLayer)
                currentOffset -= 4;
            else
                currentOffset -= 3;
        }

        //Prepare the byte that tells the game which bytes should be analyzed because they get repeated
        byte rle_Control = 0;
        for (int bit_index = 0; bit_index < 8; bit_index++)
        {
            if (rle_Length[bit_index] > 0)  //Only the pixels that repeat at least once must be set up
                rle_Control |= (byte)(1 << bit_index);     //Bitwise OR assignment
        }
        encodedArray.Add(rle_Control);
        
        //Copy the pixels to the encoded array and the RLE length if applicable
        for (int pixel_index = 0; pixel_index < 8; pixel_index++)
        {
            try
            {
                encodedArray.AddRange(pixelsToCopy[pixel_index]);
            }
            catch (ArgumentNullException)
            {
                break;
            }

            if (rle_Length[pixel_index] > 0)
            {
                byte rle_Length_HIGH = (byte)(rle_Length[pixel_index] >> 8);   // Extract the high byte of rle_Length[pixel_index]
                byte rle_Length_LOW = (byte)(rle_Length[pixel_index] & 0xFF); // Extract the low byte of rle_Length[pixel_index]

                encodedArray.Add(rle_Length_HIGH);
                encodedArray.Add(rle_Length_LOW);
            }
        }

        return encodedArray.ToArray();
    }

    /// <summary>
    /// Helper function that reads pixels from RawScheme.Data.
    /// </summary>
    /// <param name="offset">Offset where to read from the RawScheme.Data</param>
    /// <returns></returns>
    byte[] ReadPixel(ref int offset)
    {
        byte[] pixel = [];
        if (RawScheme.Properties.HasAlphaLayer)
            pixel = new byte[4];
        else
            pixel = new byte[3];
        for (int i = 0; i < pixel.Length; i++)
            pixel[i] = RawScheme.Data[offset++];
        return pixel;
    }
}