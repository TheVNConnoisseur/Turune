using System;
using System.Collections.Generic;
using System.Text;
using static Turune.Models.Utils.PixelFormat;

namespace Turune.Models.Utils;

public static class ImageFile
{
    public class BMP
    {
        //Structure taken from: https://learn.microsoft.com/en-us/windows/win32/gdi/bitmap-header-types
        //File header section: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapfileheader
        private struct BITMAPFILEHEADER()
        {
            public readonly string bfType = "BM";
            public uint bfSize;  //The size of the BMP file in bytes (including this same header)
            public readonly short bfReserved1 = 0;
            public readonly short bfReserved2 = 0;
            public uint bfOffBits; //Offset where the image data starts
        }
        
        //DIB header section: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapv5header
        private struct BITMAPV5HEADER()
        {
            public readonly uint bV5Size = 124;
            public int bV5Width;    //Width in pixels of the bitmap
            public int bV5Height;   //Height in pixels of the bitmap
            public readonly short bV5Planes = 1;
            public ushort bV5BitCount;   //Bits per pixel (BPP), which is basically the color depth of the image.
            public uint bV5Compression; //Compression method used
            public uint bV5SizeImage;   //Size of the raw bitmap (that means excluding all of the headers)
            public readonly uint bV5XPelsPerMeter = 0;
            public readonly uint bV5YPelsPerMeter = 0;
            public readonly uint bV5ClrUsed = 0;
            public readonly uint bV5ClrImportant = 0;
            public uint bV5RedMask; //Color mask that specifies the red component of each pixel
            public uint bV5GreenMask; //Color mask that specifies the green component of each pixel
            public uint bV5BlueMask; //Color mask that specifies the blue component of each pixel
            public uint bV5AlphaMask; //Color mask that specifies the alpha component of each pixel
            public readonly uint bV5CSType = 0x73524742;
            public readonly byte[] bV5Endpoints = new byte[36];
            public readonly uint bV5GammaRed = 0;
            public readonly uint bV5GammaGreen = 0;
            public readonly uint bV5GammaBlue = 0;
            public readonly uint bV5Intent = 4; 
            public readonly uint bV5ProfileData = 0;
            public readonly uint bV5ProfileSize = 0;
            public readonly uint bV5Reserved = 0;
        }

        /// <summary>
        /// Generates a BMP file stream.
        /// </summary>
        /// <param name="bitmapData">Array of the bitmap data, already bottom-up.</param>
        /// <param name="pixelFormat">Instance of the struct of the PixelFormat the image has.</param>
        /// <param name="width">Width in pixels of the bitmap.</param>
        /// <param name="height">Height in pixels of the bitmap.</param>
        /// <returns>A byte array with the BMP file.</returns>
        public static byte[] Write(byte[] bitmapData, IBmpFormatInfo pixelFormat, int width, int height)
        {
            List<byte> outputBMP = [];

            uint initialRowSize = (uint)((pixelFormat.bV5BitCount * width + 7) / 8);
            uint finalRowSize = (uint)(((pixelFormat.bV5BitCount * width + 31) / 32) * 4);
            uint finalBitmapDataSize = (uint)(finalRowSize * height);

            var fileHeader = new BITMAPFILEHEADER
            {
                bfSize = 14 + 124 + finalBitmapDataSize,   //BITMAPFILEHEADER + BITMAPV5HEADER + Bitmap data aligned
                bfOffBits = 14 + 124    //BITMAPFILEHEADER + BITMAPV5HEADER
            };
            
            outputBMP.AddRange(Encoding.ASCII.GetBytes(fileHeader.bfType));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfSize));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfReserved1));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfReserved2));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfOffBits));

            var v5Header = new BITMAPV5HEADER
            {
                bV5Width = width,
                bV5Height = height,
                bV5BitCount = pixelFormat.bV5BitCount,
                bV5Compression = pixelFormat.bV5Compression,
                bV5SizeImage = pixelFormat.bV5SizeImage,
                bV5RedMask = pixelFormat.bV5RedMask,
                bV5GreenMask = pixelFormat.bV5GreenMask,
                bV5BlueMask = pixelFormat.bV5BlueMask,
                bV5AlphaMask = pixelFormat.bV5AlphaMask,
            };

            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Size));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Width));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Height));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Planes));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5BitCount));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Compression));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5SizeImage));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5XPelsPerMeter));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5YPelsPerMeter));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5ClrUsed));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5ClrImportant));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5RedMask));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5GreenMask));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5BlueMask));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5AlphaMask));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5CSType));
            outputBMP.AddRange(v5Header.bV5Endpoints);
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5GammaRed));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5GammaGreen));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5GammaBlue));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Intent));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5ProfileData));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5ProfileSize));
            outputBMP.AddRange(BitConverter.GetBytes(v5Header.bV5Reserved));

            for (int row = 0; row < height; row++)
            {
                byte[] rowData = new byte[finalRowSize];
                Buffer.BlockCopy(bitmapData, (int)(row * initialRowSize), rowData, 0, (int)initialRowSize);
                outputBMP.AddRange(rowData);
            }

            return outputBMP.ToArray();
        }
    }
}