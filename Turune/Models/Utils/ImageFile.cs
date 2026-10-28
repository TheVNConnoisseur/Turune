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

    public struct BMP1
    {


        /// <summary>
        /// Writes a 24bpp BMP file from a tightly-packed B,G,R byte buffer.
        /// Assumes pixelData is already bottom-up (row 0 = bottom of image),
        /// which matches BMP's default row order -- no flipping needed.
        /// </summary>
        public static void WriteBmp(string filePath, byte[] pixelData, int width, int height)
        {
            int bytesPerPixel = 3;
            int rowSizeUnpadded = width * bytesPerPixel;
            int rowPadding = (4 - (rowSizeUnpadded % 4)) % 4; // pad each row to a multiple of 4 bytes
            int rowSizePadded = rowSizeUnpadded + rowPadding;
            int pixelArraySize = rowSizePadded * height;

            int fileHeaderSize = 14;
            int infoHeaderSize = 40;
            int pixelDataOffset = fileHeaderSize + infoHeaderSize;
            int fileSize = pixelDataOffset + pixelArraySize;

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // ----- BITMAPFILEHEADER (14 bytes) -----
                bw.Write((byte)'B');
                bw.Write((byte)'M');
                bw.Write(fileSize);          // total file size
                bw.Write((short)0);          // reserved1
                bw.Write((short)0);          // reserved2
                bw.Write(pixelDataOffset);   // offset to pixel data

                // ----- BITMAPINFOHEADER (40 bytes) -----
                bw.Write(infoHeaderSize);    // header size
                bw.Write(width);             // width
                bw.Write(height);            // height (positive = bottom-up)
                bw.Write((short)1);          // planes, always 1
                bw.Write((short)24);         // bits per pixel
                bw.Write(0);                 // compression = BI_RGB (none)
                bw.Write(pixelArraySize);    // image data size
                bw.Write(2835);              // horizontal resolution (px/m) ~72 DPI
                bw.Write(2835);              // vertical resolution (px/m)
                bw.Write(0);                 // colors in palette (0 = default)
                bw.Write(0);                 // important colors (0 = all)

                // ----- Pixel data, row by row, each padded to a multiple of 4 -----
                byte[] padding = new byte[rowPadding];
                for (int row = 0; row < height; row++)
                {
                    int rowStart = row * rowSizeUnpadded;
                    bw.Write(pixelData, rowStart, rowSizeUnpadded);
                    if (rowPadding > 0)
                        bw.Write(padding);
                }
            }
        }

        /// <summary>
        /// Writes a 32bpp BGRA BMP file. Expects pixelData packed as B,G,R,A per pixel,
        /// already bottom-up (row 0 = bottom of image), matching BMP's default row order.
        /// </summary>
        public static void WriteBmp32(string filePath, byte[] pixelData, int width, int height)
        {
            int bytesPerPixel = 4;
            int rowSize = width * bytesPerPixel; // always a multiple of 4, no padding needed
            int pixelArraySize = rowSize * height;

            int fileHeaderSize = 14;
            int infoHeaderSize = 40;
            int pixelDataOffset = fileHeaderSize + infoHeaderSize;
            int fileSize = pixelDataOffset + pixelArraySize;

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // ----- BITMAPFILEHEADER (14 bytes) -----
                bw.Write((byte)'B');
                bw.Write((byte)'M');
                bw.Write(fileSize);
                bw.Write((short)0);          // reserved1
                bw.Write((short)0);          // reserved2
                bw.Write(pixelDataOffset);

                // ----- BITMAPINFOHEADER (40 bytes) -----
                bw.Write(infoHeaderSize);
                bw.Write(width);
                bw.Write(height);            // positive = bottom-up
                bw.Write((short)1);          // planes
                bw.Write((short)32);         // bits per pixel
                bw.Write(0);                 // compression = BI_RGB (plain, alpha ignored by most viewers)
                bw.Write(pixelArraySize);
                bw.Write(2835);              // horizontal resolution (px/m)
                bw.Write(2835);              // vertical resolution (px/m)
                bw.Write(0);                 // colors in palette
                bw.Write(0);                 // important colors

                // ----- Pixel data, row by row (no padding needed at 32bpp) -----
                bw.Write(pixelData, 0, pixelArraySize);
            }
        }
    }
}