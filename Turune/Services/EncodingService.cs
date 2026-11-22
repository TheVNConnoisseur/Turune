using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Turune.Services;

/// <summary>
/// A single named encoding entry in the catalog. It follows the basic structure of the original class (but with the Encoding element added in):
/// https://learn.microsoft.com/en-us/dotnet/api/system.text.encodinginfo
/// </summary>
public sealed record EncodingInfo(string Name, string DisplayName, int CodePage, Encoding Encoding);

public interface IEncodingService
{
    IReadOnlyList<EncodingInfo> AvailableEncodings { get; }
}

public sealed class EncodingService : IEncodingService
{
    private static readonly Lock _registerLock = new();
    private static bool _providersRegistered;

    //Helper to ensure that the EncodingProvider is already registered to avoid any exception
    public static void EnsureProvidersRegistered()
    {
        if (_providersRegistered) return;
        lock (_registerLock)
        {
            if (_providersRegistered) return;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _providersRegistered = true;
        }
    }
    
    public IReadOnlyList<EncodingInfo> AvailableEncodings { get; }
    private readonly Dictionary<string, EncodingInfo> _byName;

    public EncodingService()
    {
        EnsureProvidersRegistered();
        AvailableEncodings = BuildCatalog();
        _byName = AvailableEncodings.ToDictionary(e => e.Name);
    }

    /// <summary>
    /// Create a catalog of all available encodings to the user's disposal.
    /// </summary>
    /// <returns>A strict-only list of all available encodings.</returns>
    private static List<EncodingInfo> BuildCatalog()
    {
        var encodingList = new List<EncodingInfo>();

        //Add the encodings that by default .NET returns: https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getencodings
        foreach (var info in Encoding.GetEncodings())
        {
            Encoding enc;
            try
            {
                //For the program, only allow lossless conversions of encodings.
                enc = Encoding.GetEncoding(info.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            }
            catch
            {
                continue;
            }

            encodingList.Add(new EncodingInfo(info.Name, info.DisplayName, info.CodePage, enc));
        }
        return encodingList.OrderBy(e => e.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
    }
}