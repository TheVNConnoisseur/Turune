using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Turune.Models.Utils;

public static class ImageFile
{
    public class BMP
    {
        //Structure taken from: https://learn.microsoft.com/en-us/windows/win32/gdi/bitmap-header-types
        //File header section: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapfileheader
        private struct BITMAPFILEHEADER()
        {
            public readonly string bfType = "BM";  //Magic number
            public uint bfSize;  //The size of the BMP file in bytes (including this same header)
            public readonly short bfReserved1 = 0; //Reserved
            public readonly short bfReserved2 = 0; //Reserved
            public uint bfOffBits; //Offset where the image data starts
        }

        //DIB Header section: https://learn.microsoft.com/en-us/windows/win32/wmdm/-bitmapinfoheader
        private struct BITMAPINFOHEADER()
        {
            public readonly uint biSize = 40;  //Size of this header, 40 bytes
            public int biWidth;   //Width in pixels of the bitmap
            public int biHeight;  //Height in pixels of the bitmap
            public readonly short biPlanes = 1;  //Number of color planes, it is always 1
            public short biBitCount;    //Bits per pixel (BPP), which is basically the color depth of the image. The only possible values are 1, 2, 4, 8, 16, 24 and 32.
            public uint biCompression; //Compression method used
            public uint biSizeImage;   //Size of the raw bitmap (that means excluding all of the headers)
            public uint biXPelsPerMeter;   //Intended horizontal resolution on the target device in pixels/meter
            public uint biYPelsPerMeter;   //Intended vertical resolution on the target device in pixels/meter
            public uint biClrUsed; //The number of color table entries used by the bitmap, or, for 16 bpp and higher, the size of a color table used for palette optimization only
            public uint biClrImportant;    //The number of color table entries considered important for display (0 if all of them are considered important)
        }
        


        /// <summary>
        /// Generates a BMP uncompressed file stream.
        /// </summary>
        /// <param name="bitmapData">Array of the bitmap data, already bottom-up.</param>
        /// <param name="bpp">Bits per pixel of the image.</param>
        /// <param name="width">Width in pixels of the bitmap.</param>
        /// <param name="height">Height in pixels of the bitmap.</param>
        /// <returns>A byte array with the BMP file.</returns>
        public static byte[] Write(byte[] bitmapData, short bpp, int width, int height)
        {
            List<byte> outputBMP = [];

            uint initialRowSize = (uint)((bpp * width + 7) / 8);
            uint finalRowSize = (uint)(((bpp * width + 31) / 32) * 4);
            uint finalBitmapDataSize = (uint)(finalRowSize * height);

            var fileHeader = new BITMAPFILEHEADER
            {
                bfSize = 14 + 40 + finalBitmapDataSize,   //BITMAPFILEHEADER + BITMAPINFOHEADER + Bitmap data aligned
                bfOffBits = 14 + 40    //BITMAPFILEHEADER + BITMAPINFOHEADER
            };
            
            outputBMP.AddRange(Encoding.ASCII.GetBytes(fileHeader.bfType));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfSize));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfReserved1));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfReserved2));
            outputBMP.AddRange(BitConverter.GetBytes(fileHeader.bfOffBits));

            var infoHeader = new BITMAPINFOHEADER
            {
                biWidth = width,
                biHeight = height,
                biBitCount = bpp,
                biCompression = 0,  //For simplicity's sake, only uncompressed images will be done
                biSizeImage = 0,    //If biCompression is set to 0 (BI_RGB), this is set to 0 too
                biXPelsPerMeter = 0,    //Unspecified
                biYPelsPerMeter = 0,    //Unspecified
                biClrUsed = 0,  //Using the maximum number of colors corresponding to the value of the biBitCount 
                biClrImportant = 0, //All colours are required
            };

            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biSize));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biWidth));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biHeight));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biPlanes));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biBitCount));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biCompression));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biSizeImage));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biXPelsPerMeter));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biYPelsPerMeter));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biClrUsed));
            outputBMP.AddRange(BitConverter.GetBytes(infoHeader.biClrImportant));

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