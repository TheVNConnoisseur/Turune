using System;

namespace Turune.Models.Utils;

//While it's not a 1:1 replica of the version offered by Microsoft (https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.pixelformats), the idea is similar.
//Avalonia itself also offers the same thing (https://docs.avaloniaui.net/api/avalonia/platform/pixelformat), but it barely offers any format.
public static class PixelFormat
{
    public class ARGB32()
    {
        public readonly struct BMP : IBmpFormatInfo
        {
            public ushort bV5BitCount { get; } = 32;
            public uint bV5Compression { get; } = 3; //BI_BITFIELDS
            public uint bV5SizeImage { get; } = 0;
            public uint bV5RedMask { get; } = 0x00FF0000;
            public uint bV5GreenMask { get; } = 0x0000FF00;
            public uint bV5BlueMask { get; } = 0x000000FF;
            public uint bV5AlphaMask { get; } = 0xFF000000;

            public BMP() { }

        }
        public readonly struct PNG : IPngFormatInfo
        {
            //placeholder

            public PNG() { }
        }
    }

    public class RGB24()
    {
        public readonly struct BMP : IBmpFormatInfo
        {
            public ushort bV5BitCount { get; } = 24;
            public uint bV5Compression { get; } = 0; //BI_RGB
            public uint bV5SizeImage { get; } = 0;
            public uint bV5RedMask { get; } = 0;
            public uint bV5GreenMask { get; } = 0;
            public uint bV5BlueMask { get; } = 0;
            public uint bV5AlphaMask { get; } = 0;

            public BMP() { }

        }
        public readonly struct PNG : IPngFormatInfo
        {
            //placeholder

            public PNG() { }
        }
    }

    public interface IBmpFormatInfo
    {
        ushort bV5BitCount { get; }
        uint bV5Compression { get; }
        uint bV5SizeImage { get; }
        uint bV5RedMask { get; }
        uint bV5GreenMask { get; }
        uint bV5BlueMask { get; }
        uint bV5AlphaMask { get; }
    }
    public interface IPngFormatInfo
    {
        //placeholder
    }
}
