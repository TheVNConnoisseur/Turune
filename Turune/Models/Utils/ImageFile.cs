using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Media.Imaging;
using static Turune.Models.Utils.PixelFormat;

namespace Turune.Models.Utils;

public static class ImageFile
{
    public class BMP
    {
        public BITMAPFILEHEADER FileHeader;
        public BITMAPCOREHEADER? CoreHeader;
        public BITMAPINFOHEADER? InfoHeader;
        public BITMAPV4HEADER? V4Header;
        public BITMAPV5HEADER? V5Header;
        public byte[] PixelData = [];
        
        //Structure taken from: https://learn.microsoft.com/en-us/windows/win32/gdi/bitmap-header-types
        //Taken from: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapfileheader
        public struct BITMAPFILEHEADER()
        {
            public readonly string bfType = "BM";
            public uint bfSize;  //The size of the BMP file in bytes (including this same header)
            public readonly short bfReserved1 = 0;
            public readonly short bfReserved2 = 0;
            public uint bfOffBits; //Offset where the image data starts
        }
        
        //Taken from: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapcoreheader
        public struct BITMAPCOREHEADER()
        {
            public readonly uint bcSize = 12; //Size in bytes of this header
            public int bcWidth;    //Width in pixels of the bitmap
            public int bcHeight;   //Height in pixels of the bitmap
            public readonly short bcPlanes = 1;
            public ushort bcBitCount;   //Bits per pixel (BPP), which is basically the color depth of the image.
        }
        
        //Extension of BITMAPCOREHEADER: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader
        public struct BITMAPINFOHEADER()
        {
            public readonly uint biSize = 40; //Size in bytes of this header
            public int biWidth;    //Width in pixels of the bitmap
            public int biHeight;   //Height in pixels of the bitmap
            public readonly short biPlanes = 1;
            public ushort biBitCount;   //Bits per pixel (BPP), which is basically the color depth of the image.
            public uint biCompression; //Compression method used
            public uint biSizeImage;   //Size of the raw bitmap (that means excluding all of the headers)
            public uint biXPelsPerMeter = 0;
            public uint biYPelsPerMeter = 0;
            public uint biClrUsed = 0;
            public uint biClrImportant = 0;
        }

        //Extension of BITMAPINFOHEADER: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapv4header
        public struct BITMAPV4HEADER()
        {
            public readonly uint bV4Size = 108; //Size in bytes of this header
            public int bV4Width;    //Width in pixels of the bitmap
            public int bV4Height;   //Height in pixels of the bitmap
            public readonly short bV4Planes = 1;
            public ushort bV4BitCount;   //Bits per pixel (BPP), which is basically the color depth of the image.
            public uint bV4Compression; //Compression method used
            public uint bV4SizeImage;   //Size of the raw bitmap (that means excluding all of the headers)
            public uint bV4XPelsPerMeter = 0;
            public uint bV4YPelsPerMeter = 0;
            public uint bV4ClrUsed = 0;
            public uint bV4ClrImportant = 0;
            public uint bV4RedMask; //Color mask that specifies the red component of each pixel
            public uint bV4GreenMask; //Color mask that specifies the green component of each pixel
            public uint bV4BlueMask; //Color mask that specifies the blue component of each pixel
            public uint bV4AlphaMask; //Color mask that specifies the alpha component of each pixel
            public uint bV4CSType = 0x73524742;
            public byte[] bV4Endpoints = new byte[36];
            public uint bV4GammaRed = 0;
            public uint bV4GammaGreen = 0;
            public uint bV4GammaBlue = 0;
        }

        //Extension of BITMAPV4HEADER: https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapv5header
        public struct BITMAPV5HEADER()
        {
            public readonly uint bV5Size = 124; //Size in bytes of this header
            public int bV5Width;    //Width in pixels of the bitmap
            public int bV5Height;   //Height in pixels of the bitmap
            public readonly short bV5Planes = 1;
            public ushort bV5BitCount;   //Bits per pixel (BPP), which is basically the color depth of the image.
            public uint bV5Compression; //Compression method used
            public uint bV5SizeImage;   //Size of the raw bitmap (that means excluding all of the headers)
            public uint bV5XPelsPerMeter = 0;
            public uint bV5YPelsPerMeter = 0;
            public uint bV5ClrUsed = 0;
            public uint bV5ClrImportant = 0;
            public uint bV5RedMask; //Color mask that specifies the red component of each pixel
            public uint bV5GreenMask; //Color mask that specifies the green component of each pixel
            public uint bV5BlueMask; //Color mask that specifies the blue component of each pixel
            public uint bV5AlphaMask; //Color mask that specifies the alpha component of each pixel
            public uint bV5CSType = 0x73524742;
            public byte[] bV5Endpoints = new byte[36];
            public uint bV5GammaRed = 0;
            public uint bV5GammaGreen = 0;
            public uint bV5GammaBlue = 0;
            public uint bV5Intent = 4; 
            public uint bV5ProfileData = 0;
            public uint bV5ProfileSize = 0;
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
        /// <remarks>The BITMAPV5HEADER is used.</remarks>
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
    
        /// <summary>
        /// Parse a BMP file stream, and obtain all of the metadata that goes alongside wth it.
        /// </summary>
        /// <param name="bitmapData"></param>
        /// <returns>A BMP object with all of its metadata parsed.</returns>
        /// <exception cref="ArgumentException">The file is not a valid Bitmap file.</exception>
        /// <exception cref="InvalidDataException">The DIB header size is not a valid one.</exception>
        public static BMP Read(byte[] bitmapData)
        {
            if (!Validate(bitmapData))
                throw new ArgumentException("The file is not a valid BMP file.");

            var bmp = new BMP();
            int currentOffset = 0;

            currentOffset += 2; //Skip bfType

            uint bfSize = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
            currentOffset += 4;

            currentOffset += 4; //Skip bfReserved1 and bfReserved2

            uint bfOffBits = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
            currentOffset += 4;

            bmp.FileHeader = new BITMAPFILEHEADER
            {
                bfSize = bfSize,
                bfOffBits = bfOffBits,
            };

            uint bcSize = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
            currentOffset += 4;
            switch(bcSize)
            {
                case 12:
                    {
                        int bcWidth = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        int bcHeight = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        currentOffset += 2; //Skip bcPlanes

                        ushort bcBitCount = BinaryPrimitives.ReadUInt16LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 2;

                        bmp.CoreHeader = new BITMAPCOREHEADER
                        {
                            bcWidth = bcWidth,
                            bcHeight = bcHeight,
                            bcBitCount = bcBitCount,
                        };
                        break;
                    }
                case 40:
                    {
                        int biWidth = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        int biHeight = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        currentOffset += 2; //Skip biPlanes

                        ushort biBitCount = BinaryPrimitives.ReadUInt16LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 2;

                        uint biCompression = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint biSizeImage = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint biXPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint biYPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint biClrUsed = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint biClrImportant = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        bmp.InfoHeader = new BITMAPINFOHEADER
                        {
                            biWidth = biWidth,
                            biHeight = biHeight,
                            biBitCount = biBitCount,
                            biCompression = biCompression,
                            biSizeImage = biSizeImage,
                            biXPelsPerMeter = biXPelsPerMeter,
                            biYPelsPerMeter = biYPelsPerMeter,
                            biClrUsed = biClrUsed,
                            biClrImportant = biClrImportant,
                        };
                        break;
                    }
                case 108:
                    {
                        int bV4Width = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        int bV4Height = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        currentOffset += 2; //Skip bV4Planes

                        ushort bV4BitCount = BinaryPrimitives.ReadUInt16LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 2;

                        uint bV4Compression = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4SizeImage = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4XPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4YPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4ClrUsed = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4ClrImportant = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4RedMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4GreenMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4BlueMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4AlphaMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4CSType = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        byte[] bV4Endpoints = bitmapData.AsSpan(currentOffset, 36).ToArray();
                        currentOffset += 36;

                        uint bV4GammaRed = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4GammaGreen = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV4GammaBlue = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        bmp.V4Header = new BITMAPV4HEADER
                        {
                            bV4Width = bV4Width,
                            bV4Height = bV4Height,
                            bV4BitCount = bV4BitCount,
                            bV4Compression = bV4Compression,
                            bV4SizeImage = bV4SizeImage,
                            bV4XPelsPerMeter = bV4XPelsPerMeter,
                            bV4YPelsPerMeter = bV4YPelsPerMeter,
                            bV4ClrUsed = bV4ClrUsed,
                            bV4ClrImportant = bV4ClrImportant,
                            bV4RedMask = bV4RedMask,
                            bV4GreenMask = bV4GreenMask,
                            bV4BlueMask = bV4BlueMask,
                            bV4AlphaMask = bV4AlphaMask,
                            bV4CSType = bV4CSType,
                            bV4Endpoints = bV4Endpoints,
                            bV4GammaRed = bV4GammaRed,
                            bV4GammaGreen = bV4GammaGreen,
                            bV4GammaBlue = bV4GammaBlue,
                        };
                        break;
                    }
                case 124:
                    {
                        int bV5Width = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        int bV5Height = BinaryPrimitives.ReadInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        currentOffset += 2; //Skip bV5Planes

                        ushort bV5BitCount = BinaryPrimitives.ReadUInt16LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 2;
                        
                        uint bV5Compression = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5SizeImage = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5XPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5YPelsPerMeter = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5ClrUsed = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5ClrImportant = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5RedMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5GreenMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5BlueMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5AlphaMask = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5CSType = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        byte[] bV5Endpoints = bitmapData.AsSpan(currentOffset, 36).ToArray();
                        currentOffset += 36;

                        uint bV5GammaRed = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5GammaGreen = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5GammaBlue = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5Intent = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5ProfileData = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        uint bV5ProfileSize = BinaryPrimitives.ReadUInt32LittleEndian(bitmapData.AsSpan(currentOffset));
                        currentOffset += 4;

                        currentOffset += 4; //Skip bV5Reserved

                        bmp.V5Header = new BITMAPV5HEADER
                        {
                            bV5Width = bV5Width,
                            bV5Height = bV5Height,
                            bV5BitCount = bV5BitCount,
                            bV5Compression = bV5Compression,
                            bV5SizeImage = bV5SizeImage,
                            bV5XPelsPerMeter = bV5XPelsPerMeter,
                            bV5YPelsPerMeter = bV5YPelsPerMeter,
                            bV5ClrUsed = bV5ClrUsed,
                            bV5ClrImportant = bV5ClrImportant,
                            bV5RedMask = bV5RedMask,
                            bV5GreenMask = bV5GreenMask,
                            bV5BlueMask = bV5BlueMask,
                            bV5AlphaMask = bV5AlphaMask,
                            bV5CSType = bV5CSType,
                            bV5Endpoints = bV5Endpoints,
                            bV5GammaRed = bV5GammaRed,
                            bV5GammaGreen = bV5GammaGreen,
                            bV5GammaBlue = bV5GammaBlue,
                            bV5Intent = bV5Intent,
                            bV5ProfileData = bV5ProfileData,
                            bV5ProfileSize = bV5ProfileSize,
                        };
                        break;
                    }
                default:
                    throw new InvalidDataException($"Unrecognized DIB header size: {bcSize}");
            }

            bmp.PixelData = bitmapData.AsSpan((int)bfOffBits).ToArray();

            return bmp;
        }

        /// <summary>
        /// Validates a given BMP file.
        /// </summary>
        /// <param name="bitmapData">Array of the bitmap data.</param>
        /// <returns>True if the file is valid BMP file, false if otherwise.</returns>
        public static bool Validate(byte[] bitmapData)
        {
            //Check the data region and see if the file even loads as a Bitmap to begin with
            using var stream = new MemoryStream(bitmapData);
            try
            {
                Bitmap testBitmap = new(stream);
                testBitmap.Dispose();
            }
            catch (ArgumentException)
            {
                return false;
            }
            finally
            {
                stream.Dispose();
            }
            
            //Now check the bfType header
            int currentOffset = 0;
            if (bitmapData[currentOffset++] != (byte)'B' || bitmapData[currentOffset++] != (byte)'M')
                return false;

            return true;
        }
    }
}